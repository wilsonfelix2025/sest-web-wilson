using System.Threading.Tasks;
using SestWeb.Domain.Enums;

namespace SestWeb.Application.UseCases.RegistrosEventosUseCases.EditarRegistrosEventos
{
    public interface IEditarRegistrosEventosUseCase
    {
        Task<EditarRegistrosEventosOutput> Execute(string idPoço, TipoRegistroEvento tipo, TrechoRegistroEventoInput[] trechos, MarcadorRegistroEventoInput[] marcadores);
    }
}