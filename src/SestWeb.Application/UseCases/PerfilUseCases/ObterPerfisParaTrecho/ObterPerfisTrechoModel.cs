
using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisParaTrecho
{
    public class ObterPerfisTrechoModel
    {
        public List<PerfilModel> ListaPerfis { get; set; }
        public double PMTopo { get; set; }
        public double PMBase { get; set; }
    }
}
