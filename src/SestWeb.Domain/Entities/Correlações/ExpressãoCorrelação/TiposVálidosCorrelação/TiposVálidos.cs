using System.Collections.Generic;
using System.Linq;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.TiposVálidosCorrelação
{
    public static class TiposVálidos
    {
        public static readonly List<string> Perfis;

        static TiposVálidos()
        {
            //Perfis = PerfilOld.GetTiposPerfil().Select(t => t.Mnemônico).ToList();
            Perfis = TiposPerfil.Todos.Select(t => t.Mnemônico).ToList();
        }
    }
}
