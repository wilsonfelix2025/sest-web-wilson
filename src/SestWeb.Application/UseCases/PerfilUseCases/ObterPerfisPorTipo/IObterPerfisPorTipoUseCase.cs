using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisPorTipo
{
    public interface IObterPerfisPorTipoUseCase
    {
        Task<ObterPerfisPorTipoOutput> Execute(string idPoço, string mnemônico);
    }
}