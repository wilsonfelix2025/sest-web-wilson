using System.Collections.Generic;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Domain.Importadores.Leitores.PoçoWeb
{
    public class LeitorObjetivosPoçoWeb
    {
        private readonly PoçoWebDto _poçoWeb;
        public List<ObjetivoDTO> Objetivos { get; private set; }
        private bool _buscarDadosShallow;
        private Poço _poçoAtual;

        public LeitorObjetivosPoçoWeb(PoçoWebDto poçoWeb, bool buscarDadosShallow, Poço PoçoAtual)
        {
            _poçoWeb = poçoWeb;
            _buscarDadosShallow = buscarDadosShallow;
            _poçoAtual = PoçoAtual;
            Objetivos = new List<ObjetivoDTO>();
            GetObjetivos();
        }

        private void GetObjetivos()
        {
            foreach (var objetivos in _poçoWeb.Revision.Content.Input.Objectives.Targets)
            {
                var valorPm = objetivos.TopMd;
                var tipo = string.Empty;

                if (objetivos.Type.ToLower() == "secundary")
                    tipo = "Secundário";
                else
                    tipo = "Primário";

                if (valorPm != null)
                {
                    Objetivos.Add(new ObjetivoDTO(valorPm,
                        tipo));
                }

                if (_buscarDadosShallow)
                    break;
            }
        }
    }
}
