using MongoDB.Bson.Serialization;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeEntrada;
using SestWeb.Domain.Entities.Cálculos.Base.PerfisDeSaída;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Base;
using SestWeb.Domain.Entities.Correlações.ParâmetroCorrelação;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Métodos
{
    public class CálculoPressãoPorosGradienteInterpretado: CálculoPressãoPoros, ICálculoPressãoPoros
    {
        public CálculoPressãoPorosGradienteInterpretado(string nome, GrupoCálculo grupoCálculo, CorrelaçãoPressãoPoros métodoCálculo, IPerfisEntrada perfisEntrada,
            IPerfisSaída perfisSaída, IList<ParâmetroCorrelação> parâmetros, Trajetória trajetória, ILitologia litologia, DadosGerais dadosGerais) : base(nome, grupoCálculo, métodoCálculo, perfisEntrada, perfisSaída, parâmetros, trajetória, litologia, dadosGerais)
        {

        }

        public override void Execute(bool chamadaPelaPipeline)
        {
            double pmInicial, pmFinal, valorInicial, valorFinal;
            var perfilEntrada = PerfisEntrada.Perfis.FirstOrDefault(x => x.Mnemonico == TiposPerfil.GeTipoPerfil<PPORO>().Mnemônico);
            var perfilSaída = PerfisSaída.Perfis.First(x => x.Mnemonico == TiposPerfil.GeTipoPerfil<GPPI>().Mnemônico);
            var fatorConversão = 5.8674;

            if (ParâmetrosCorrelação.Count() > 0)
            {
                var gn = ParâmetrosCorrelação.Single(x => x.NomeParâmetro == nameof(Gn)).Valor;

                pmInicial = ObterProfundidadeInicial();
                pmFinal = Trajetória.ÚltimoPonto.Pm.Valor;
                valorInicial = gn;
                valorFinal = gn;
            }
            else if (perfilEntrada != null)
            {
                pmInicial = perfilEntrada.PrimeiroPonto.Pm.Valor;
                pmFinal = perfilEntrada.UltimoPonto.Pm.Valor;
                valorInicial = ((perfilEntrada.PrimeiroPonto.Valor - PressãoAtmosférica) * fatorConversão) / (perfilEntrada.PrimeiroPonto.Pv.Valor - ObterProfundidadeReferência());
                valorFinal = ((perfilEntrada.UltimoPonto.Valor - PressãoAtmosférica) * fatorConversão) / (perfilEntrada.UltimoPonto.Pv.Valor - ObterProfundidadeReferência());
            } 
            else
            {
                throw new Exception("é necessário fornecer Gn ou um perfil de entrada para o gradiente.");
            }

            perfilSaída.Clear();
            perfilSaída.AddPontoEmPm(ConversorProfundidade, pmInicial, valorInicial, TipoProfundidade.PM, OrigemPonto.Calculado);
            perfilSaída.AddPontoEmPm(ConversorProfundidade, pmFinal, valorFinal, TipoProfundidade.PM, OrigemPonto.Calculado);
        }

        public override List<PerfilBase> GetPerfisEntradaSemPontos()
        {
            throw new NotImplementedException();
        }

        public override List<string> GetTiposPerfisEntradaFaltantes()
        {
            throw new NotImplementedException();
        }

        public override PerfilBase ObterPerfilComTrend()
        {
            throw new NotImplementedException();
        }

        public override PerfilBase ObterPerfilObservado()
        {
            throw new NotImplementedException();
        }

        private double ObterProfundidadeReferência()
        {
            switch (Geometria.CategoriaPoço)
            {
                case CategoriaPoço.OffShore:
                    return Geometria.MesaRotativa;
                case CategoriaPoço.OnShore:
                    return ObterProfundidadeInicial();
                default: throw new InvalidOperationException("Categoria de poço não reconhecida!");
            }
        }

        private double ObterProfundidadeInicial()
        {
            switch (Geometria.CategoriaPoço)
            {
                case CategoriaPoço.OffShore:
                    return Geometria.MesaRotativa + Geometria.OffShore.LaminaDagua;
                case CategoriaPoço.OnShore:
                    return Geometria.MesaRotativa + Geometria.OnShore.AlturaDeAntePoço + Geometria.OnShore.LençolFreático;

                default: throw new InvalidOperationException("Categoria de poço não reconhecida!");
            }
        }

        public new static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(CálculoPressãoPorosGradienteInterpretado)))
                return;

            BsonClassMap.RegisterClassMap<CálculoPressãoPorosGradienteInterpretado>(calcPPoros =>
            {
                calcPPoros.SetDiscriminator(nameof(CálculoPressãoPorosGradienteInterpretado));
            });
        }
    }
}
