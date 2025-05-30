using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMec
{
    public interface ICriarRelacionamentoCorrsPropMecUseCase
    {
        Task<CriarRelacionamentoCorrsPropMecOutput> Execute(string idPoço,
            string grupoLitológico, string nomeAutor, string chaveAutor, string corrUcs,
            string corrCoesa, string corrAngat, string corrRestr);
    }
}
