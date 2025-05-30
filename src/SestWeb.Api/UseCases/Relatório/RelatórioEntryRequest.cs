using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Relatório
{
    public class RelatórioEntryRequest
    {
        public List<string> Curvas { get; set; }
        public string Data { get; set; }
        public List<string> Registros { get; set; }
        public string Titulo { get; set; }
    }
}
