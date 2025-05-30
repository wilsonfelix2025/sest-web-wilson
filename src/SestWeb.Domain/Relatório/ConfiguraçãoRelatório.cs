using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Domain.Relatório
{
    public class ConfiguraçãoRelatório
    {
        public bool MostrarNome { get; set; }
        public bool MostrarTipoPoço { get; set; }
        public bool MostrarProfundidadeFinal { get; set; }
        public bool MostrarMR { get; set; }
        public bool MostrarLDA { get; set; }
        public TrackRelatório Trajetória { get; set; }
        public TrackRelatório Litologia { get; set; }
        public List<TrackRelatório> Estratigrafias { get; set; }
        public List<TrackRelatório> Graficos { get; set; }
    }
}
