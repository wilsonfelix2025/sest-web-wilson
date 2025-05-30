using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.GradienteSobrecarga.Factory;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using System;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.PontosEntity;

namespace SestWeb.Domain.Entities.Cálculos.GradienteSobrecarga
{
    public class CálculoGradienteSobrecarga : Cálculo, ICálculoGradienteSobrecarga
    {
        private const double ConvGrad = 0.170433;

        #region Constructor

        private CálculoGradienteSobrecarga(string nome, GrupoCálculo grupoCálculo, IPerfisEntrada perfisEntrada, IPerfisSaída perfisSaída, Trajetória trajetória, ILitologia litologia) : base(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia)
        {
        }

        public static void RegisterCálculoGradienteSobrecargaCtor()
        {
            CálculoGradienteSobrecargaFactory.RegisterCálculoGradienteSobrecargaCtor((nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia) => new CálculoGradienteSobrecarga(nome, grupoCálculo, perfisEntrada, perfisSaída, trajetória, litologia));
        }

        #endregion

        #region Methods

        public override void Execute(bool chamadaPelaPipeline)
        {
            var pontosTVert = PerfisEntrada.Perfis.First().GetPontos();
            var gradienteSobrecarga = PerfisSaída.Perfis.First();
            gradienteSobrecarga.Clear();

            for (int index = 0; index < pontosTVert.Count; index++)
            {
                double gsobr = pontosTVert[index].Valor / (ConvGrad * pontosTVert[index].Pv.Valor);

                if (gsobr < 0) continue;

                gradienteSobrecarga.AddPontoEmPm(ConversorProfundidade, pontosTVert[index].Pm, gsobr, TipoProfundidade.PM, OrigemPonto.Calculado);
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
    }
}
