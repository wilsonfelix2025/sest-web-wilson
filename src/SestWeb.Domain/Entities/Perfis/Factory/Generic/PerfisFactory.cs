using System;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Perfis.Factory.Generic
{
    public class PerfisFactory<T> : PerfisFactory where T : PerfilBase
    {
        private PerfisFactory() { }

        public static T Create(string nome, IConversorProfundidade conversor, ILitologia litologia)
        {
            return (T) Create(typeof(T).Name, nome, conversor, litologia);
        }

        public static void Register(Func<string, IConversorProfundidade, ILitologia, PerfilBase> ctor)
        {
            var mnemônico = typeof(T).Name;

            if (!PerfisConstructors.ContainsKey(mnemônico))
                PerfisConstructors.Add(mnemônico, ctor);
        }
    }
}
