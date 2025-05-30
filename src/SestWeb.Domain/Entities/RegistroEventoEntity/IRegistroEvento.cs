
using System.Collections.Generic;
using MongoDB.Bson;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.SistemasUnidades.Grupos.Base;

namespace SestWeb.Domain.Entities.RegistrosEventos
{
    public interface IRegistroEvento
    {
        ObjectId Id { get; }

        string IdPoço { get; }

        string Nome { get; }

        GrupoUnidades GrupoDeUnidades { get; }

        string Unidade { get; }
        object ValorPadrão { get; }

        EstiloVisual EstiloVisual { get; }

        IReadOnlyList<TrechoRegistroEvento> Trechos { get; }

        void ResetTrechos();
        void AddTrecho(double pmTopo, double pmBase, double pvTopo, double pvBase, string comentário, IConversorProfundidade conversorProfundidade);
        void AddTrecho(double pm, double pv, double valor, string comentário, IConversorProfundidade conversorProfundidade);
        void SetEstiloVisual(string marcador, string corDoMarcador, string contornoDoMarcador);

    }
}
