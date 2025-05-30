using MongoDB.Bson.Serialization.Attributes;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory.Generic;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.EstilosVisuais;
using SestWeb.Domain.SistemasUnidades.Grupos;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;

namespace SestWeb.Domain.Entities.Perfis.TiposPerfil
{
    public class THORmax : PerfilBase, IPerfilDerived
    {
        static THORmax()
        {
            PerfisFactory<THORmax>.Register((nome, conversorProfundidade, litologia) => new THORmax(nome, conversorProfundidade, litologia));
            GrupoUnidades.RegisterTipoPerfil<GrupoUnidadesDePressão>(_tipoPerfil);
            GrupoPerfis.RegisterTipoPerfil(_tipoPerfil, _grupoPerfis);
        }

        [BsonConstructor]
        private THORmax(string nome, IConversorProfundidade conversorProfundidade, ILitologia litologia) : base(nome, conversorProfundidade, litologia)
        {
            EstiloVisual = new EstiloVisual("#00bb99", 1.0, EstiloLinha.Solid, TipoMarcador.Nenhum, "Transparent");
        }

        #region Fields

        public static readonly TipoPerfil _tipoPerfil = Base.TiposPerfil.GeTipoPerfil<THORmax>();

        public static readonly string _descrição = "Tensão Horizontal Maior";

        public static readonly GrupoPerfis _grupoPerfis = GrupoPerfis.Tensões;

        public static readonly GrupoUnidades _grupoUnidades = GrupoUnidades.GetGrupoUnidades<GrupoUnidadesDePressão>();

        #endregion

        #region Properties

        public override TipoPerfil TipoPerfil { get; } = _tipoPerfil;

        public override string Mnemonico { get; private protected set; } = _tipoPerfil.Mnemônico;

        public override string Descrição { get; private protected set; } = _descrição;

        public override GrupoPerfis GrupoPerfis { get; private protected set; } = _grupoPerfis;

        public override GrupoUnidades GrupoDeUnidades { get; private protected set; } = _grupoUnidades;

        public override string UnidadePadrão { get; } = _grupoUnidades.UnidadePadrão.Símbolo;

        public override bool PodeSerConvertidoParaGradiente { get; private protected set; } = true;


        #endregion
    }
}