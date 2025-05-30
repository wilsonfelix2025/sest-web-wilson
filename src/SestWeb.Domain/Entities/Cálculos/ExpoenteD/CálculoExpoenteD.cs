using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD.Correlação;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD.ExpoenteDCalulator;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD.ExpoenteDCorrigidoCalculator;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD.Factory;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.ExpoenteD
{

    public sealed class CálculoExpoenteD : Cálculo, ICálculoExpoenteD
    {
        #region Constructor

        private CálculoExpoenteD(string nome, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, Trajetória trajetória, ILitologia litologia, CorrelaçãoExpoenteD correlação) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            _correlação = correlação;
            ZerarPerfisSaída();
        }

        public static void RegisterCálculoExpoenteDCtor()
        {
            CálculoExpoenteDFactory.RegisterCálculoExpoenteDCtor((nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, correlação) => new CálculoExpoenteD(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, correlação));
        }

        #endregion

        #region Fields

        private CorrelaçãoExpoenteD _correlação;

        #endregion

        #region Methods

        public override void Execute(bool chamadaPelaPipeline)
        {
            VerificarLitologia();
            Entities.Perfis.TiposPerfil.ExpoenteD perfilSaída = (Entities.Perfis.TiposPerfil.ExpoenteD)PerfisSaída.Perfis[0];

            var calculador = _correlação == CorrelaçãoExpoenteD.CalculadorExpoenteDCorrigido ? new CarculadorExpoenteDCorrigido() : new CalculadorExpoenteD();
            calculador.Calcular(PerfisEntrada, ref perfilSaída, Litologia, ConversorProfundidade);
        }

        private void VerificarLitologia()
        {
            if (base.Litologia == null || !base.Litologia.ContémPontos())
            {
                throw new ArgumentException(
                    $"Cálculo {Nome} realizou tentativa de acessar Litologia sem dados.");
            }
        }

        public override List<string> GetTiposPerfisEntradaFaltantes()
        {
            throw new NotImplementedException();
        }

        public override List<PerfilBase> GetPerfisEntradaSemPontos()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Map
        public new static void Map()
        {
            BsonClassMap.RegisterClassMap<CálculoExpoenteD>(calc =>
            {
                calc.AutoMap();
                calc.MapMember(p => p._correlação).SetSerializer(new EnumSerializer<CorrelaçãoExpoenteD>(BsonType.String)); ;
            });

        }
        #endregion
    }
}
