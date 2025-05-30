using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory.Generic;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.SistemasUnidades.Grupos;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;

namespace SestWeb.Domain.Entities.Perfis.TiposPerfil
{
    public class RESTR : PerfilBase, IPerfilDerived
    {
        static RESTR()
        {
            PerfisFactory<RESTR>.Register((nome, conversorProfundidade, litologia) => new RESTR(nome, conversorProfundidade, litologia));
            GrupoUnidades.RegisterTipoPerfil<GrupoUnidadesDePressão>(_tipoPerfil);
            GrupoPerfis.RegisterTipoPerfil(_tipoPerfil, _grupoPerfis);
        }

        [BsonConstructor]
        private RESTR(string nome, IConversorProfundidade conversorProfundidade, ILitologia litologia) : base(nome, conversorProfundidade, litologia)
        {
        }

        #region Fields

        public static readonly TipoPerfil _tipoPerfil = Base.TiposPerfil.GeTipoPerfil<RESTR>();

        public static readonly string _descrição = "Resistência à Tração";

        public static readonly GrupoPerfis _grupoPerfis = GrupoPerfis.PropriedadesMecanicas;

        public static readonly GrupoUnidades _grupoUnidades = GrupoUnidades.GetGrupoUnidades<GrupoUnidadesDePressão>();

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