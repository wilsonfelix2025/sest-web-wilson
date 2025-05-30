using SestWeb.Domain.DTOs.Importação;
using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Importadores.Leitores.PoçoWeb
{
    public class LeitorSapatasPoçoWeb
    {
        private readonly PoçoWebDto _poçoWeb;
        public List<SapataDTO> Sapatas { get; private set; }
        private bool _buscarDadosShallow;

        public LeitorSapatasPoçoWeb(PoçoWebDto poçoWeb, bool buscarDadosShallow)
        {
            _poçoWeb = poçoWeb;
            _buscarDadosShallow = buscarDadosShallow;
            Sapatas = new List<SapataDTO>();
            GetSapatas();
        }

        private void GetSapatas()
        {
            foreach (var fases in _poçoWeb.Revision.Content.Input.Phases)
            {
                var pm = fases.Columns?.The1?.FinalMd;
                var diam = fases.Columns?.The1?.Values.FirstOrDefault()?.Material.Od?.ToString();

                if (pm != null && diam != null)
                {
                    Sapatas.Add(new SapataDTO
                    {
                        Pm = pm,
                        Diâmetro = diam
                    });
                }

                if (_buscarDadosShallow)
                    break;
            }
        }
    }
}
