using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Depleção.Casos;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Depleção
{
    public class CalculadorDepleção
    {
        private Dictionary<CasosDepleção, DepleçãoBase> _casosDepleções;
        private readonly List<DadosDepleção> _dadosDepleção;
        //psi -> lb/gal
        private const double FatorConversãoPsiToLbgal = 0.1704;

        public CalculadorDepleção(List<DadosDepleção> dadosDepleção)
        {
            _dadosDepleção = dadosDepleção;
        }

        private void PrepararCasos(DadosDepleção dadosDepleção)
        {
            _casosDepleções = new Dictionary<CasosDepleção, DepleçãoBase>
            {
                {CasosDepleção.Caso1, new DepleçãoCaso01(dadosDepleção)},
                {CasosDepleção.Caso2, new DepleçãoCaso02(dadosDepleção)},
                {CasosDepleção.Caso3, new DepleçãoCaso03(dadosDepleção)},
                {CasosDepleção.Caso7, new DepleçãoCaso07(dadosDepleção)},
                {CasosDepleção.Caso8, new DepleçãoCaso08(dadosDepleção)},
                {CasosDepleção.Caso9, new DepleçãoCaso09(dadosDepleção)},
                {CasosDepleção.Caso4, new DepleçãoCaso04(dadosDepleção)},
                {CasosDepleção.Caso5, new DepleçãoCaso05(dadosDepleção)},
                {CasosDepleção.Caso6, new DepleçãoCaso06(dadosDepleção)},
                {CasosDepleção.Caso10, new DepleçãoCaso10(dadosDepleção)},
                {CasosDepleção.Caso11, new DepleçãoCaso11(dadosDepleção)},
                {CasosDepleção.Caso12, new DepleçãoCaso12(dadosDepleção)}
            };
        }

        private double ObterPressãoDepletada(Profundidade pvi, DadosDepleção dados)
        {
            PrepararCasos(dados);

            if (Equals(pvi, dados.Reservatório.ContatoGásÓleo))
            {
                pvi = new Profundidade(pvi.Valor + 0.1);
            }

            if (Equals(pvi, dados.Reservatório.ContatoÓleoÁgua))
            {
                pvi = new Profundidade(pvi.Valor + 0.1);
            }

            var caso = dados.ObterCaso(pvi);
            return _casosDepleções[caso].ObterPressãoDepletada(pvi);
        }

        public List<Ponto> ObterPontosGporoDepletada(IList<Ponto> pontos)
        {
            var pontosDepletados = new List<Ponto>();
            var pontosLista = (List<Ponto>)pontos;
            pontosLista.Sort((a, b) => a.Pm.Valor - b.Pm.Valor > 0.01 ? 1 : -1);

            for (var i = 0; i < pontos.Count; i++)
            {
                var ponto = pontos[i];

                foreach (var dadoDepleção in _dadosDepleção)
                {
                    if (ponto.Pv.Valor >= dadoDepleção.Reservatório.ProfundidadeTopo.Valor &&
                        ponto.Pv.Valor <= dadoDepleção.Reservatório.ProfundidadeBase.Valor)
                    {
                        var pressãoDepletada = ObterPressãoDepletada(ponto.Pv, dadoDepleção);
                        var valorPontoDepletado = pressãoDepletada / (ponto.Pv.Valor * FatorConversãoPsiToLbgal);
                        var novoPonto = new Ponto(ponto.Pm, ponto.Pv, valorPontoDepletado, ponto.TipoProfundidade, OrigemPonto.Calculado, ponto.TipoRocha.Mnemonico);
                        pontosDepletados.Add(novoPonto);
                    }
                }

                pontosDepletados.Add(ponto);
            }

            return pontosDepletados;
        }
    }
}
