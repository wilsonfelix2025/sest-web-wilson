using System;
using System.Collections.Generic;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Perfis.Factory
{
    public class PerfisFactory
    {
        private protected static readonly Dictionary<string, Func<string, IConversorProfundidade, ILitologia, PerfilBase>> PerfisConstructors
            = new Dictionary<string, Func<string, IConversorProfundidade, ILitologia, PerfilBase>>();

        public static PerfilBase Create(string mnemônico, string nome, IConversorProfundidade conversor, ILitologia litologia)
        {
            if (PerfisConstructors.TryGetValue(mnemônico, out Func<string, IConversorProfundidade, ILitologia, PerfilBase> constructor))
                return constructor(nome, conversor, litologia);

            throw new ArgumentException($"Não existe perfil registrado com o mnemônico: {mnemônico}");
        }
    }
}
