using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.SapataUseCases.CriarSapata
{
    public interface ICriarSapataUseCase
    {
        Task<CriarSapataOutput> Execute(string idPoço, double profundidadeMedida, string diâmetroSapata);
    }
}