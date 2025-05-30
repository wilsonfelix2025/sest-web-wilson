using System.Threading.Tasks;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;

namespace SestWeb.Application.UseCases.PoçoUseCases.CriarPoço
{
    public interface ICriarPoçoUseCase
    {
        Task<CriarPoçoOutput> Execute(string nome, TipoPoço tipoPoço);
    }
}