using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.RegistrosEventos
{
    public interface ITrechoRegistroEvento
    {
        PontoRegistroEvento Topo { get; }
        PontoRegistroEvento Base { get; }

        object Valor { get; }
        string Comentário { get; }

        void Edit(double newValue);

        void AtualizarPV(IConversorProfundidade conversor);

        void ConverterParaPv();

        void ConverterParaPM();

        void Shift(double delta);
    }
}
