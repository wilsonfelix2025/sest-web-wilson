using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçõesPorTipo
{
    public interface IObterCorrelaçõesPorTipoUseCase
    {
        Task<ObterCorrelaçõesPorTipoOutput> Execute(string idPoço, string mnemônico);
    }
}
