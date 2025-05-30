using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory.Generic;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.SistemasUnidades.Grupos;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;

namespace SestWeb.Domain.Entities.Perfis.TiposPerfil
{
    public class AZTHmin : PerfilBase, IPerfilDerived
    {
        static AZTHmin()
        {
            PerfisFactory<AZTHmin>.Register((nome, conversorProfundidade, litologia) => new AZTHmin(nome, conversorProfundidade, litologia));
            GrupoUnidades.RegisterTipoPerfil<GrupoUnidadesDeÂngulo>(_tipoPerfil);
            GrupoPerfis.RegisterTipoPerfil(_tipoPerfil, _grupoPerfis);
        }

        [BsonConstructor]
        private AZTHmin(string nome, IConversorProfundidade conversorProfundidade, ILitologia litologia) : base(nome, conversorProfundidade, litologia)
        {
        }

        #region Fields

        public static readonly TipoPerfil _tipoPerfil = Base.TiposPerfil.GeTipoPerfil<AZTHmin>();

        public static readonly string _descrição = "Azimute da Tensão Horizontal Menor";

        public static readonly GrupoPerfis _grupoPerfis = GrupoPerfis.Tensões;

        public static readonly GrupoUnidades _grupoUnidades = GrupoUnidades.GetGrupoUnidades<GrupoUnidadesDeÂngulo>();

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