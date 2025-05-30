using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CriarPerfilRelaçãoTensões
{
    public interface ICriarPerfilRelaçãoTensõesUseCase
    {
        Task<CriarPerfilRelaçãoTensõesOutput> Execute(CriarPerfilRelaçãoInput input);

    }
}
