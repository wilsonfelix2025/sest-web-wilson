using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Correlação.CriarCorrelaçãoPoço
{
    public class CreateWellCorrRequest
    {
        public string idPoço { get; set; }
        public string Nome { get; set; }
        public string NomeAutor { get; set; }
        public string ChaveAutor { get; set; }
        public string Descrição { get; set; }
        public string Expressão { get; set; }
    }
}
