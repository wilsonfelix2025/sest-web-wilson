using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Domain.Relatório
{
    public class TrackRelatório
    {
        public string Titulo { get; set; }
        public string Data { get; set; }
        public List<string> Curvas { get; set; }
    }
}
