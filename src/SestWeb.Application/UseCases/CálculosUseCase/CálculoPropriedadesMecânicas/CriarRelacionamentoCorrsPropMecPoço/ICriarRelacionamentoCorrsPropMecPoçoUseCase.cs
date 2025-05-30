using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMecPoço
{
    public interface ICriarRelacionamentoCorrsPropMecPoçoUseCase
    {
        Task<CriarRelacionamentoCorrsPropMecPoçoOutput> Execute(string idPoço,
            string grupoLitológico, string nomeAutor, string chaveAutor, string corrUcs,
            string corrCoesa, string corrAngat, string corrRestr);
    }
}
