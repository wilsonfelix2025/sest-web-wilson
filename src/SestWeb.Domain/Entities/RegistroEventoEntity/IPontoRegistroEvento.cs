using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.RegistrosEventos
{
    public interface IPontoRegistroEvento
    {
        Profundidade Pm { get; }
        Profundidade Pv { get; }

        void AtualizarPV(IConversorProfundidade conversor);

        void ConverterParaPv();

        void ConverterParaPM();

        void Shift(double delta);
    }
}
