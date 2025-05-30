using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.GerarRelatórioUseCase
{
    public interface IGerarRelatórioUseCase
    {
        Task<GerarRelatórioOutput> Execute(GerarRelatórioInput input);
    }
}
