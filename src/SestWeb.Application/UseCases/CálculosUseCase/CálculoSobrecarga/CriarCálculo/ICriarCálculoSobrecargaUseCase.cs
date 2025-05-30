using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoSobrecarga.CriarCálculo
{
    public interface ICriarCálculoSobrecargaUseCase
    {
        Task<CriarCálculoSobrecargaOutput> Execute(CriarCálculoSobrecargaInput input);
    }
}
