using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.UsuárioUseCases.TrocarSenha
{
    public interface ITrocarSenhaUseCase
    {
        Task<TrocarSenhaUseCaseOutput> Execute(string idUsuario, string senhaAntiga, string novaSenha);
    }
}
