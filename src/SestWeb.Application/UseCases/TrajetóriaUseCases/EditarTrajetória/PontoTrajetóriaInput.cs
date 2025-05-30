using SestWeb.Domain.Entities.ProfundidadeEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.TrajetóriaUseCases.EditarTrajetória
{
    public class PontoTrajetóriaInput
    {
        public double Pm { get; set; }
        public double Inclinação { get; set; }
        public double Azimute { get; set; }
    }
}
