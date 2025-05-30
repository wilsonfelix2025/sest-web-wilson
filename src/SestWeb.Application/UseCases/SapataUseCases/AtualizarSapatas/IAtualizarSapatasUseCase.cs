using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.SapataUseCases.AtualizarSapatas
{
    public interface IAtualizarSapatasUseCase
    {
        Task<AtualizarSapatasOutput> Execute(string idPoço, AtualizarSapatasInput input);
    }
}
