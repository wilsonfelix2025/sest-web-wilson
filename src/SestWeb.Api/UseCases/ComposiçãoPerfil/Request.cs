using System.Collections.Generic;

namespace SestWeb.Api.UseCases.ComposiçãoPerfil
{
    public class Request
    {
        public string IdPoço { get; set; }
        public string NomePerfil { get; set; }
        public string TipoPerfil { get; set; }
        public List<ComporPerfilRequest> ListaTrechos { get; set; }

    }
}
