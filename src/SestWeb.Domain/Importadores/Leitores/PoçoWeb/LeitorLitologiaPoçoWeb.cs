using System.Globalization;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;

namespace SestWeb.Domain.Importadores.Leitores.PoçoWeb
{
    public class LeitorLitologiaPoçoWeb
    {
        private readonly PoçoWebDto _poçoWeb;
        public LitologiaDTO Litologia { get; private set; }
        private readonly bool _buscarDadosShallow;
        private Poço _poçoAtual;
        private readonly double _baseSedimentos;

        public LeitorLitologiaPoçoWeb(PoçoWebDto poçoWeb, bool buscarDadosShallow, Poço PoçoAtual, double baseSedimentos)
        {
            _poçoWeb = poçoWeb;
            _buscarDadosShallow = buscarDadosShallow;
            _poçoAtual = PoçoAtual;
            _baseSedimentos = baseSedimentos;
            Litologia = new LitologiaDTO();
            GetLitologia();
        }

        private void GetLitologia()
        {
            if (_poçoWeb.Revision.Content.Input.Lithology == null)
                return;

            bool primeiroItem = true;
            var soma = 0.0;
            var ultimaBase = 0.0;
            var nomeLitologia = string.Empty;
            bool éCota = false;

            if (_baseSedimentos == 0.0)
            {
                if (_buscarDadosShallow == false)
                    soma = _poçoAtual.ObterBaseDeSedimentos();
            }
            else
            {
                soma = _baseSedimentos;
            }

            foreach (var lito in _poçoWeb.Revision.Content.Input.Lithology.Rocks)
            {
                if (primeiroItem && _buscarDadosShallow == false)
                {
                    var primeiroPonto = new PontoLitologiaDTO { Pm = soma.ToString(CultureInfo.InvariantCulture), TipoRocha = lito.Name };

                    Litologia.Pontos.Add(primeiroPonto);

                    var profundidade = lito.BaseMd;
                    if (profundidade == null)
                    {
                        profundidade = lito.BaseElevation * -1;
                        éCota = true;
                        Litologia.TipoProfundidade = "Cota";
                    }

                    ultimaBase = profundidade.Value;
                    primeiroItem = false;
                }
                else
                {
                    var ponto = new PontoLitologiaDTO {Pm = ultimaBase.ToString(CultureInfo.InvariantCulture), TipoRocha = lito.Name};
                    Litologia.Pontos.Add(ponto);

                    var profundidade = lito.BaseMd;
                    if (éCota || profundidade == null)
                        profundidade = lito.BaseElevation * -1;

                    ultimaBase = profundidade.Value;
                    nomeLitologia = lito.Name;
                    if (_buscarDadosShallow)
                        break;
                }
            }

            var ultimoPonto = new PontoLitologiaDTO { Pm = ultimaBase.ToString(), TipoRocha = nomeLitologia };
            Litologia.Pontos.Add(ultimoPonto);

            if (_buscarDadosShallow)
                Litologia.Classificação = "Litologia";
            else
                Litologia.Classificação = _poçoAtual.TipoPoço == TipoPoço.Projeto || _poçoAtual.TipoPoço == TipoPoço.Monitoramento ? "Prevista" : "Interpretada";

            Litologia.Nome = "Litologia";
        }

    }
}
