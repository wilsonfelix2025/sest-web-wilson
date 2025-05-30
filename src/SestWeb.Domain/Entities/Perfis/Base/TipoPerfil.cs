using System;

namespace SestWeb.Domain.Entities.Perfis.Base
{
    public class TipoPerfil
    {
        public TipoPerfil(Type type)
        {
            Type = type;
        }

        public string Mnemônico => Type.Name;

        public Type Type { get; }
    }
}