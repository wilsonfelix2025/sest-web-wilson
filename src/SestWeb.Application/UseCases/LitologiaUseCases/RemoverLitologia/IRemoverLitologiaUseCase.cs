using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.LitologiaUseCases.RemoverLitologia
{
    public interface IRemoverLitologiaUseCase
    {
        Task<RemoverLitologiaOutput> Execute(string idPoço, string idLitologia);
    }
}