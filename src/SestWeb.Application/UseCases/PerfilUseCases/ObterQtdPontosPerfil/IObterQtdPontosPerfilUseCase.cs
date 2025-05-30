
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterQtdPontosPerfil
{
    public interface IObterQtdPontosPerfilUseCase
    {
        Task<ObterQtdPontosPerfilOutput> Execute(string idPerfil, ObterQtdPontosPerfilInput input);

    }
}
