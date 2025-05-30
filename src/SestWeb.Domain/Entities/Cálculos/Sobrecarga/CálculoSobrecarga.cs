using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.GradienteSobrecarga;
using SestWeb.Domain.Entities.Cálculos.Sobrecarga.Factory;
using SestWeb.Domain.Entities.Cálculos.TensãoVertical;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Sobrecarga
{
    public class CálculoSobrecarga : Cálculo, ICálculoSobrecarga
    {
        #region Constructor

        private readonly ICálculoTensãoVertical _tvert;
        private readonly ICálculoGradienteSobrecarga _gSobr;

        private CálculoSobrecarga(string nome, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, 
            IPerfisSaída perfisSaída, Trajetória trajetória, ILitologia litologia, ICálculoTensãoVertical cálculoTensãoVertical,
            ICálculoGradienteSobrecarga cálculoGradienteSobrecarga) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
            _tvert = cálculoTensãoVertical;
            _gSobr = cálculoGradienteSobrecarga;
        }

        public static void RegisterCálculoSobrecargaCtor()
        {
            CálculoSobrecargaFactory.RegisterCálculoSobrecargaCtor((nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, tvert, gsobr) => new CálculoSobrecarga(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia, tvert, gsobr));
        }

        #endregion

        #region Methods

        public override void Execute(bool chamadaPelaPipeline)
        {
            if (!PerfisEntradaPossuemPontos)
                return;

            ZerarPerfisSaída();
            Calcular();
        } 

        private void Calcular()
        {
            _tvert.Execute();

            _gSobr.PerfisEntrada.Perfis.Clear();
            _gSobr.PerfisEntrada.Perfis.Add(_tvert.PerfisSaída.Perfis.First());
            _gSobr.Execute();
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
    }
}
