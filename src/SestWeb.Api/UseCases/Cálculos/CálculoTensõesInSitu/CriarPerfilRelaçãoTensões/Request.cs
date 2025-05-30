
using System.Collections.Generic;

namespace SestWeb.Api.UseCases.Cálculos.CálculoTensõesInSitu.CriarPerfilRelaçãoTensões
{
    public class Request
    {
        public List<ValoresRequest> Valores { get; set; }
        public string IdPoço { get; set; }
        public string NomePerfil { get; set; }
    }
}
