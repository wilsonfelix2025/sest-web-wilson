using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.ExportarRegistros
{
    public class ExportarRegistrosRequest
    {
        public List<string> Registros { get; set; }
        public string IdPoço { get; set; }
    }
}
