using System.Threading.Tasks;

namespace SestWeb.Application.DebugUseCases.AlimentarBancoComPocoWeb
{
    public interface IAlimentarBancoComPoçoWebUseCase
    {
        Task<string> Execute(string authorization);
    }
}