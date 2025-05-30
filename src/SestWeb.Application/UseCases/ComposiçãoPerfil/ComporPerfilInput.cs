
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SestWeb.Application.UseCases.ComposiçãoPerfil
{
    public class ComporPerfilInput
    {
        public string NomePerfil { get; set; }
        public string TipoPerfil { get; set; }
        public List<ComposiçãoPerfilListaInput> Lista { get; set; } = new List<ComposiçãoPerfilListaInput>();
    }
}
