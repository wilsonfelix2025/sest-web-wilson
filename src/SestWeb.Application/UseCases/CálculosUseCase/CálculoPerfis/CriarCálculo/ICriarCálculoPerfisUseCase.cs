using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.CriarCálculo
{
    public interface ICriarCálculoPerfisUseCase
    {
        Task<CriarCálculoPerfisOutput> Execute(CriarCalculoPerfisInput input);
    }
}
