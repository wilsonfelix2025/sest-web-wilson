using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.RelatórioUseCases
{
    public class RelatórioEntryInput
    {
        public List<string> Curvas { get; set; }
        public string Data { get; set; }
        public List<string> Registros { get; set; }
        public string Título { get; set; }
    }
}
