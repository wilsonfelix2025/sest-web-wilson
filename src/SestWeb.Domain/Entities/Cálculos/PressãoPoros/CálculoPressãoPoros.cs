using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Base;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Factory;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Métodos;
using SestWeb.Domain.Entities.Correlações.ParâmetroCorrelação;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros
{
    public class CálculoPressãoPoros : CálculoPressãoPorosBase, ICálculoPressãoPoros
    {
        #region Constructor

        public CálculoPressãoPoros(string nome, GrupoCálculo grupoCálculo, CorrelaçãoPressãoPoros métodoCálculo, IPerfisEntrada perfisEntrada,
            IPerfisSaída perfisSaída, IList<ParâmetroCorrelação> parâmetros, Trajetória trajetória, ILitologia litologia, DadosGerais dadosGerais) : base(nome, grupoCálculo, métodoCálculo, perfisEntrada, perfisSaída, parâmetros, trajetória, litologia, dadosGerais)
        {
        }

        public static void RegisterCálculoPressãoPorosCtor()
        {
            CálculoPressãoPorosFactory.RegisterCálculoPressãoPorosCtor((nome, grupoCálculo, métodoCálculo, perfisEntrada, perfisSaída, parâmetros, trajetória, litologia, dadosGerais) => { 
                
                return new CálculoPressãoPoros(nome, grupoCálculo, métodoCálculo, perfisEntrada, perfisSaída, parâmetros, trajetória, litologia, dadosGerais); 
            });
        }

        #endregion

        #region Methods

        public override void Execute(bool chamadaPelaPipeline)
        {
            throw new NotImplementedException();
        }

        public override List<string> GetTiposPerfisEntradaFaltantes()
        {
            throw new NotImplementedException();
        }

        public override List<PerfilBase> GetPerfisEntradaSemPontos()
        {
            throw new NotImplementedException();
        }

        protected override double CalcularGporo(double profundidadePm, double profundidadePv, double valorSobrecarga, double valor, double valorPerfil, double exp, Func<double, double> getGppn, GrupoLitologico nomeLitologia, ConcurrentBag<Tuple<double, double>> profundidadesForaLitologia)
        {
            throw new NotImplementedException();
        }

        public override PerfilBase ObterPerfilObservado()
        {
            throw new NotImplementedException();
        }

        public override PerfilBase ObterPerfilComTrend()
        {
            throw new NotImplementedException();
        }

        #endregion
        public new static void Map()
        {
            CálculoPressãoPorosEatonDTCFiltrado.Map();
            CálculoPressãoPorosEatonExpoenteDFiltrado.Map();
            CálculoPressãoPorosEatonResistividadeFiltrado.Map();
            CálculoPressãoPorosHidrostática.Map();
            CálculoPressãoPorosGradienteInterpretado.Map();
        }
    }
}
