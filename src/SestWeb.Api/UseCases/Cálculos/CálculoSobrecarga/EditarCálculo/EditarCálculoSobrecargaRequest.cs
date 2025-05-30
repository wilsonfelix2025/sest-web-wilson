using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Cálculos.CálculoSobrecarga.EditarCálculo
{
    public class EditarCálculoSobrecargaRequest
    {
        public string IdPoço { get; set; }
        public string IdRhob { get; set; }
        public string Nome { get; set; }

        public string IdCálculoAntigo { get; set; }
    }
}
