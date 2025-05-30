using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.PerfilUseCases.CriarPerfil
{
    public interface ICriarPerfilUseCase
    {
        Task<CriarPerfilOutput> Execute(string idPoço, string nome, string mnemônico);
    }
}