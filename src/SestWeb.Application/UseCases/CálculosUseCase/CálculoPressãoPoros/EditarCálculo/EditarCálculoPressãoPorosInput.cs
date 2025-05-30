using SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.Base;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.ReservatórioInput;
using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.EditarCálculo
{
    public class EditarCálculoPressãoPorosInput: CálculoPressãoPorosInput
    {
        public string IdCálculo { get; set; }
    }
}
