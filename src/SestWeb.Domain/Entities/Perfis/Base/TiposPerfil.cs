using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;

namespace SestWeb.Domain.Entities.Perfis.Base
{
    public static class TiposPerfil
    {
        private static Dictionary<string, TipoPerfil> PerfilDerivedTypes;

        static TiposPerfil()
        {
            LoadDerivedTypes();
            InicializaMnemônicosExtras();
        }

        private static void LoadDerivedTypes()
        {
            PerfilDerivedTypes =
                Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(PerfilBase)))
                    .ToDictionary(t => t.Name, t => new TipoPerfil(t));
        }

        public static IReadOnlyList<TipoPerfil> Todos => PerfilDerivedTypes.Values.ToImmutableList();
        public static IReadOnlyList<string> TodosString => PerfilDerivedTypes.Keys.ToImmutableList();

        public static bool GeTipoPerfil(string mnemônico, out TipoPerfil tipoPerfil)
        {
            tipoPerfil = default;

            if (string.IsNullOrWhiteSpace(mnemônico))
                return false;

            if (!PerfilDerivedTypes.TryGetValue(mnemônico, out tipoPerfil))
                PerfilDerivedTypes.TryGetValue(nameof(GENERICO), out tipoPerfil);

            return true;
        }

        public static TipoPerfil GeTipoPerfil(string mnemônico)
        {
            return GeTipoPerfil(mnemônico, out TipoPerfil tipoPerfil) ? tipoPerfil : default;
        }

        public static TipoPerfil GeTipoPerfil<T>()
        {
            var mnemônico = typeof(T).Name;
            return GeTipoPerfil(mnemônico);
        }

        private static void InicializaMnemônicosExtras()
        {
            // Mnemônicos Extras
            PerfilDerivedTypes.Add("BITSIZE", GeTipoPerfil(nameof(DIAM_BROCA)));

            // Mnenônicos SEST 5
            PerfilDerivedTypes.Add("DENF", GeTipoPerfil(nameof(RHOB)));
            PerfilDerivedTypes.Add("DENG", GeTipoPerfil(nameof(RHOG)));
            PerfilDerivedTypes.Add("REST", GeTipoPerfil(nameof(RESIST)));
            PerfilDerivedTypes.Add("RESCMP", GeTipoPerfil(nameof(UCS)));
            PerfilDerivedTypes.Add("THORm", GeTipoPerfil(nameof(THORmin)));
            PerfilDerivedTypes.Add("THORM", GeTipoPerfil(nameof(THORmax)));
            PerfilDerivedTypes.Add("AZTHM", GeTipoPerfil(nameof(AZTHmin)));

            // Mnemônicos LAS
            PerfilDerivedTypes.Add("GR", GeTipoPerfil(nameof(GRAY)));
            PerfilDerivedTypes.Add("DT", GeTipoPerfil(nameof(DTC)));
            PerfilDerivedTypes.Add("CALI", GeTipoPerfil(nameof(CALIP)));
            PerfilDerivedTypes.Add("CAL", GeTipoPerfil(nameof(CALIP)));
            PerfilDerivedTypes.Add("ILD", GeTipoPerfil(nameof(RESIST)));
            PerfilDerivedTypes.Add("ILM", GeTipoPerfil(nameof(RESIST)));
            PerfilDerivedTypes.Add("IL", GeTipoPerfil(nameof(RESIST)));
            PerfilDerivedTypes.Add("RES", GeTipoPerfil(nameof(RESIST)));
            PerfilDerivedTypes.Add("AZIM", GeTipoPerfil(nameof(AZTHmin)));
            PerfilDerivedTypes.Add("AZI", GeTipoPerfil(nameof(AZTHmin)));
            PerfilDerivedTypes.Add("BS", GeTipoPerfil(nameof(DIAM_BROCA)));
            PerfilDerivedTypes.Add("DENS", GeTipoPerfil(nameof(RHOB)));
            PerfilDerivedTypes.Add("RHO", GeTipoPerfil(nameof(RHOB)));
            PerfilDerivedTypes.Add("RH", GeTipoPerfil(nameof(RHOB)));
            PerfilDerivedTypes.Add("CC", GeTipoPerfil(nameof(RHOB)));
            PerfilDerivedTypes.Add("DEN", GeTipoPerfil(nameof(RHOB)));
            PerfilDerivedTypes.Add("PHI", GeTipoPerfil(nameof(PORO)));
            PerfilDerivedTypes.Add("POR", GeTipoPerfil(nameof(PORO)));
            PerfilDerivedTypes.Add("ROPI", GeTipoPerfil(nameof(IROP)));
            PerfilDerivedTypes.Add("RPI", GeTipoPerfil(nameof(IROP)));
            PerfilDerivedTypes.Add("ECD", GeTipoPerfil(nameof(GECD)));
            PerfilDerivedTypes.Add("WOH", GeTipoPerfil(nameof(WOB)));
        }
    }
}
