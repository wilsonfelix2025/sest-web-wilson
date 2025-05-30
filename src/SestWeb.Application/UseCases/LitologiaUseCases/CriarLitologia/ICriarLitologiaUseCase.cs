using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.LitologiaUseCases.CriarLitologia
{
    public interface ICriarLitologiaUseCase
    {
        Task<CriarLitologiaOutput> Execute(string idPoço, string tipoLitologia);
    }
}