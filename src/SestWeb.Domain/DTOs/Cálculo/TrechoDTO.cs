using System.Collections.Generic;

namespace SestWeb.Domain.DTOs.Cálculo
{
    public class TrechoDTO
    {
        public string TipoPerfil { get; set; }
        public double PmTopo { get; set; }
        public double PmBase { get; set; }
        public string Correlação { get; set; }
        public List<VariávelDTO> ListaParametros { get; set; }
    }
}
