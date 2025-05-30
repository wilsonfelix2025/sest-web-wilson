using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.ExportarRegistros
{
    public class ExportarRegistrosInput
    {
        public List<string> Registros { get; set; }
        public string IdPoço { get; set; }
    }
}
