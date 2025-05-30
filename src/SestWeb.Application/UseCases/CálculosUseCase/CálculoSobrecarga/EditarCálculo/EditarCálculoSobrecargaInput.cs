using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoSobrecarga.EditarCálculo
{
    public class EditarCálculoSobrecargaInput
    {
        public string IdRhob { get; set; }

        public string IdPoço { get; set; }

        public string Nome { get; set; }

        public string IdCálculoAntigo { get; set; }
    }
}
