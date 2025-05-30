using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory.Generic;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.SistemasUnidades.Grupos;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;

namespace SestWeb.Domain.Entities.Perfis.TiposPerfil
{
    public class PORO : PerfilBase, IPerfilDerived
    {
        static PORO()
        {
            PerfisFactory<PORO>.Register((nome, conversorProfundidade, litologia) => new PORO(nome, conversorProfundidade, litologia));
            GrupoUnidades.RegisterTipoPerfil<GrupoUnidadesDeProporção>(_tipoPerfil);
            GrupoPerfis.RegisterTipoPerfil(_tipoPerfil, _grupoPerfis);
        }

        [BsonConstructor]
        private PORO(string nome, IConversorProfundidade conversorProfundidade, ILitologia litologia) : base(nome, conversorProfundidade, litologia)
        {
        }

        #region Fields

        public static readonly TipoPerfil _tipoPerfil = Base.TiposPerfil.GeTipoPerfil<PORO>();

        public static readonly string _descrição = "Porosidade";

        public static readonly GrupoPerfis _grupoPerfis = GrupoPerfis.Perfis;

        public static readonly GrupoUnidades _grupoUnidades = GrupoUnidades.GetGrupoUnidades<GrupoUnidadesDeProporção>();

        #endregion

        #region Properties

        public override TipoPerfil TipoPerfil { get; } = _tipoPerfil;

        public override string Mnemonico { get; private protected set; } = _tipoPerfil.Mnemônico;

        public override string Descrição { get; private protected set; } = _descrição;

        public override GrupoPerfis GrupoPerfis { get; private protected set; } = _grupoPerfis;

        public override GrupoUnidades GrupoDeUnidades { get; private protected set; } = _grupoUnidades;

        public override string UnidadePadrão { get; } = _grupoUnidades.UnidadePadrão.Símbolo;

        #endregion
    }
}