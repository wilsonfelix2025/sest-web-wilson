using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.BaixarRelatórioUseCase
{
    public interface IBaixarRelatórioUseCase
    {
        Task<BaixarRelatórioOutput> Execute(BaixarRelatórioInput input);
    }
}
