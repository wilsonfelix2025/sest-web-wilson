using System.Collections.Generic;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Domain.Importadores.Leitores.PoçoWeb
{
    public class LeitorEstratigrafiaPoçoWeb
    {
        private readonly PoçoWebDto _poçoWeb;
        public EstratigrafiaDTO Estratigrafia { get; private set; }
        private bool _buscarDadosShallow;
        private Poço _poçoAtual;
        private double _baseSedimentos;

        public LeitorEstratigrafiaPoçoWeb(PoçoWebDto poçoWeb, bool buscarDadosShallow, Poço PoçoAtual, double baseSedimentos)
        {
            _poçoWeb = poçoWeb;
            _buscarDadosShallow = buscarDadosShallow;
            _poçoAtual = PoçoAtual;
            _baseSedimentos = baseSedimentos;
            Estratigrafia = new EstratigrafiaDTO();
            GetEstratigrafia();
        }

        private void GetEstratigrafia()
        {
            var soma = 0.0;
            bool primeiraCR = true;
            bool primeiraFM = true;
            var ultimaBase = 0.0;

            if (_baseSedimentos == 0.0)
            {
                if (_buscarDadosShallow == false)
                    soma = _poçoAtual.ObterBaseDeSedimentos();
            }
            else
            {
                soma = _baseSedimentos;
            }

            foreach (var estrat in _poçoWeb.Revision.Content.Input.Lithology.Ages)
            {
                if (primeiraCR && _buscarDadosShallow == false)
                {
                    CriarEstratigrafias(soma.ToString(), estrat.Name, "CR");
                    ultimaBase = estrat.BaseMd ?? 0;
                    primeiraCR = false;
                }
                else
                {

                    CriarEstratigrafias(ultimaBase.ToString(), estrat.Name, "CR");
                    ultimaBase = estrat.BaseMd ?? 0;
                    if (_buscarDadosShallow)
                        break;
                }
            }

            foreach (var estrat in _poçoWeb.Revision.Content.Input.Lithology.Formations)
            {
                if (primeiraFM && _buscarDadosShallow == false)
                {
                    CriarEstratigrafias(soma.ToString(), estrat.Name, "FM");
                    ultimaBase = estrat.BaseMd ?? 0;
                    primeiraFM = false;
                }
                else
                {
                    CriarEstratigrafias(ultimaBase.ToString(), estrat.Name, "FM");
                    ultimaBase = estrat.BaseMd ?? 0;
                    if (_buscarDadosShallow)
                        break;
                }
            }
        }

        private void CriarEstratigrafias(string pm, string name, string tipo)
        {
            var basePm = pm;

            var lista = new List<ItemEstratigrafiaDTO>();

            var item = new ItemEstratigrafiaDTO(basePm, name, "", "", basePm);

            if (Estratigrafia.Itens.ContainsKey(tipo))
            {
                Estratigrafia.Itens[tipo].Add(item);
            }
            else
            {
                lista.Add(item);
                Estratigrafia.Itens.Add(tipo, lista);
            }
        }

    }
}
