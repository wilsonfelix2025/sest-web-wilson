using MongoDB.Bson;
using SestWeb.Domain.EstilosVisuais;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;

namespace SestWeb.Domain.Entities.Perfis.Base
{
    public interface IPerfilDerived
    {
        ObjectId Id { get; }

        string IdPoço { get; }

        string Nome { get; }

        string Mnemonico { get; }

        TipoPerfil TipoPerfil { get; }

        string Descrição { get; }

        GrupoPerfis GrupoPerfis { get; }

        GrupoUnidades GrupoDeUnidades { get; }

        string UnidadePadrão { get; }

        EstiloVisual EstiloVisual { get; }
    }
}
