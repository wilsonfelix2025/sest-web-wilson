
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.Trend;

namespace SestWeb.Domain.Validadores
{
    public class TrendValidator : AbstractValidator<PerfilBase>
    {
        public TrendValidator(Poço poço, bool éParaEditar)
        {
            RuleFor(x => x).Custom((perfil, context) =>
            {
                if (perfil.PodeTerTrendBaseFolhelho == false && perfil.PodeTerTrendCompactacao == false)
                {
                    context.AddFailure("Perfil não permite criação de trend");
                }
            });

            RuleFor(x => x.GetPontos()).Custom((pontos, context) =>
            {
                if (pontos.Count < 2)
                {
                    context.AddFailure("Perfil não possui pontos suficientes");
                }
            });

            RuleFor(x => x).Custom((perfil, context) =>
            {
                if (perfil.Trend != null && éParaEditar == false)
                {
                    context.AddFailure("Perfil já possui Trend");
                }
            });

            RuleFor(x => x.Trend).Custom((trend, context) =>
            {
                if (trend != null)
                {
                    var baseSedimentos = poço.ObterBaseDeSedimentos();
                    var primeiroTrecho = trend.PrimeiroTrecho();

                    if (primeiroTrecho.PvTopo < baseSedimentos)
                    {
                        context.AddFailure("Primeiro ponto menor que base de sedimentos");
                    }

                    
                }
            });

            RuleFor(x => x.Trend).Custom((trend, context) =>
            {
                if (trend != null)
                {
                    var últimoPontoTrajetória = poço.Trajetória.ÚltimoPonto;
                    var últimoTrecho = trend.ÚltimoTrecho();

                    if (trend.TipoTrend == TipoTrendEnum.Compactação && últimoTrecho.PvBase > últimoPontoTrajetória.Pv.Valor)
                    {
                        context.AddFailure("Último ponto maior que último ponto da trajetória");
                    }

                    if (trend.TipoTrend == TipoTrendEnum.LBF && últimoTrecho.PvBase > últimoPontoTrajetória.Pm.Valor)
                    {
                        context.AddFailure("Último ponto maior que último ponto da trajetória");
                    }

                }
            });

            RuleFor(x => x.Trend).Custom((trend, context) =>
            {
                if (trend != null)
                {
                    if (trend.Trechos.Any() && trend.Trechos.Count > 1)
                    {
                        var validação = ValidateTrechos(trend.Trechos);
                        if (!string.IsNullOrWhiteSpace(validação))
                        {
                            context.AddFailure(validação);
                        }
                    }
                }
            });
        }
        
        private string ValidateTrechos(IEnumerable<TrechoTrend> trechos)
        {
            var retorno = string.Empty;
            var primeiroTrecho = true;
            var trechoAnterior = new TrechoTrend(0, 0,0,0,0);
            foreach (var trecho in trechos.OrderBy(t => t.PvTopo))
            {
                if (primeiroTrecho == false && trecho.PvTopo != trechoAnterior.PvBase)
                {
                    retorno = "Trechos inválidos: " + trecho.PvTopo + "/" + trecho.PvBase 
                        + " - " + trechoAnterior.PvTopo + "/" + trechoAnterior.PvBase; 
                    return retorno;
                }

                primeiroTrecho = false;
                trechoAnterior = trecho;
            }

            return retorno;
        }
    }
}
