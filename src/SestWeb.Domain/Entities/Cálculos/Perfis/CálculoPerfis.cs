using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization;
using SestWeb.Domain.DTOs.Cálculo;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Composto;
using SestWeb.Domain.Entities.Cálculos.Perfis.Factory;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Cálculos.Perfis
{
    public sealed class CálculoPerfis : Cálculo, ICálculoPerfis
    {
        #region Constructor

        private CálculoPerfis(string nome, IList<ICorrelação> listaCorrelação, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, Trajetória trajetória, ILitologia litologia, IList<VariávelDTO> variáveis, IList<TrechoCálculo> trechos, ICorrelaçãoFactory correlaçãoFactory, Geometria geometria, DadosGerais dadosGerais, List<TrechoDTO> trechosFront, string correlaçãoDoCálculo) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            CorrelaçãoDoCálculo = correlaçãoDoCálculo;
            ListaCorrelação = listaCorrelação;
            Variáveis = variáveis;
            Trechos = trechos;
            TrechosFront = trechosFront;
            CorrelaçãoFactory = correlaçãoFactory;
            Geometria = geometria;
            DadosGerais = dadosGerais;
            ListaNomesCorrelação = new List<string>();
            ListaNomesCorrelação.AddRange(ListaCorrelação.Select(s => s.Nome));
            ZerarPerfisSaída();
        }

        public static void RegisterCálculoPerfisCtor()
        {
            CálculoPerfisFactory.RegisterCálculoPerfisCtor((nome, correlação, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, variáveis, trechos, correlaçãoFactory, geometria, dadosGerais, trechosFront,  correlaçãoDoCálculo) => new CálculoPerfis(nome, correlação, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, variáveis, trechos, correlaçãoFactory, geometria, dadosGerais, trechosFront, correlaçãoDoCálculo));
        }

        #endregion

        #region Properties
        private DadosGerais DadosGerais { get; }
        private Geometria Geometria { get; }
        public IList<ICorrelação> ListaCorrelação { get; }
        public List<string> ListaNomesCorrelação { get; private set; }
        public IList<VariávelDTO> Variáveis { get; private set; }
        public IList<TrechoCálculo> Trechos { get; private set; }
        public List<TrechoDTO> TrechosFront { get; private set; }
        private ICorrelaçãoFactory CorrelaçãoFactory { get; }
        public string CorrelaçãoDoCálculo { get; private set; }

        #endregion

        #region Methods

        public override void Execute(bool chamadaPelaPipeline)
        {
            var variáveis = ObterTodasVariáveisCálculo(Variáveis, ListaCorrelação);
            var cálculoComposto = new CálculoComposto(Nome, ListaCorrelação, PerfisEntrada, variáveis, Trechos, Litologia, GrupoCálculo, ConversorProfundidade, CorrelaçãoFactory, PerfisSaída, Geometria, DadosGerais, CorrelaçãoDoCálculo, true);
            CorrelaçãoDoCálculo = cálculoComposto.Correlação.Expressão.Bruta;
            cálculoComposto.Execute(chamadaPelaPipeline);
        }

        public Dictionary<string, double> ObterTodasVariáveisCálculo(IList<VariávelDTO> parâmetros,
            IList<ICorrelação> correlações)
        {
            var variáveis = new Dictionary<string, double>();

            foreach (var correlação in correlações)
            {
                var vars = ObterVariáveisDoCálculo(parâmetros, correlação);
                foreach (var variável in vars)
                {
                    variáveis.Add(variável.Key, variável.Value);
                }
            }

            return variáveis;
        }

        private Dictionary<string, double> ObterVariáveisDoCálculo(IEnumerable<VariávelDTO> parâmetros,
            ICorrelação correlação)
        {
            var variáveis = new Dictionary<string, double>();
            foreach (var parâmetro in parâmetros)
            {
                if (parâmetro.Correlação == correlação.Nome)
                {
                    variáveis.Add(parâmetro.Parâmetro, parâmetro.Valor.ToDouble());
                }
            }
            return variáveis;
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
            BsonClassMap.RegisterClassMap<CálculoPerfis>(calc =>
            {
                calc.AutoMap();
                calc.UnmapMember(p => p.ListaCorrelação);
                calc.UnmapMember(p => p.Trechos);
                calc.MapMember(p => p.Variáveis);
                calc.MapMember(p => p.ListaNomesCorrelação);
                calc.MapMember(p => p.CorrelaçãoDoCálculo);
                calc.MapMember(p => p.TrechosFront);
                // calc.MapCreator(f => new CálculoPerfis(f.Nome, f.GrupoCálculo, f.PerfisEntrada, f.PerfisSaída, null, null, f.LimiteInferior, f.LimiteSuperior, f.TipoCorte, f.DesvioMáximo));
            });

        }
        #endregion
    }
}
