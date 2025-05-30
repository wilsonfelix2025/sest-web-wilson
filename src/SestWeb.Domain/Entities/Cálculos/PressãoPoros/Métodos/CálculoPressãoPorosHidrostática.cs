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
    public class CálculoPressãoPorosHidrostática: CálculoPressãoPoros, ICálculoPressãoPoros
    {
        public CálculoPressãoPorosHidrostática(string nome, GrupoCálculo grupoCálculo, CorrelaçãoPressãoPoros métodoCálculo, IPerfisEntrada perfisEntrada,
            IPerfisSaída perfisSaída, IList<ParâmetroCorrelação> parâmetros, Trajetória trajetória, ILitologia litologia, DadosGerais dadosGerais) : base(nome, grupoCálculo, métodoCálculo, perfisEntrada, perfisSaída, parâmetros, trajetória, litologia, dadosGerais)
        {

        }

        public override void Execute(bool chamadaPelaPipeline)
        {
            var profundidadeInicial = ObterProfundidadeInicial();

            var perfilSaídaEmPressão = PerfisSaída.Perfis.First(x => x.Mnemonico == TiposPerfil.GeTipoPerfil<PPORO>().Mnemônico);
            perfilSaídaEmPressão.Clear();

            for (var i = profundidadeInicial; i < Trajetória.ÚltimoPonto.Pm.Valor; i += 1)
            {
                var profundidadePm = i;
                if (profundidadePm < profundidadeInicial)
                {
                    continue;
                }

                if (!Trajetória.TryGetTVDFromMD(profundidadePm, out var profundidadePv))
                {
                    continue;
                }

                if (!Litologia.ObterTipoRochaNaProfundidade(profundidadePm, out var tipoRocha))
                {
                    continue;
                }

                var valor = GetPressãoPorosHidrostática(profundidadePm);
                perfilSaídaEmPressão.AddPonto(ConversorProfundidade, profundidadePm, profundidadePv, valor, TipoProfundidade.PM, OrigemPonto.Calculado);
            }

            var valorFinal = GetPressãoPorosHidrostática(Trajetória.ÚltimoPonto.Pm.Valor);
            perfilSaídaEmPressão.AddPonto(ConversorProfundidade, Trajetória.ÚltimoPonto.Pm.Valor, Trajetória.ÚltimoPonto.Pv.Valor, valorFinal, TipoProfundidade.PM, OrigemPonto.Calculado);
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

        private double GetPressãoPorosHidrostática(double profundidade)
        {
            var gn = ParâmetrosCorrelação.Single(x => x.NomeParâmetro == nameof(Gn)).Valor;
            var result = Trajetória.TryGetTVDFromMD(profundidade, out var pv);

            if (!result)
            {
                throw new Exception($"Não foi possível obter o PV para o PM {profundidade} durante o cálculo.");
            }

            switch (Geometria.CategoriaPoço)
            {
                case CategoriaPoço.OffShore:
                    return (gn * (pv - Geometria.MesaRotativa) * FatorConversão) + PressãoAtmosférica;
                case CategoriaPoço.OnShore:
                    return (gn * (pv - (Geometria.MesaRotativa + Geometria.OnShore.LençolFreático + Geometria.OnShore.AlturaDeAntePoço)) * FatorConversão) + PressãoAtmosférica;
                default:
                    return 0;
            }
        }

        public new static void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(CálculoPressãoPorosHidrostática)))
                return;

            BsonClassMap.RegisterClassMap<CálculoPressãoPorosHidrostática>(calcPPoros =>
            {
                calcPPoros.SetDiscriminator(nameof(CálculoPressãoPorosHidrostática));
            });
        }
    }
}
