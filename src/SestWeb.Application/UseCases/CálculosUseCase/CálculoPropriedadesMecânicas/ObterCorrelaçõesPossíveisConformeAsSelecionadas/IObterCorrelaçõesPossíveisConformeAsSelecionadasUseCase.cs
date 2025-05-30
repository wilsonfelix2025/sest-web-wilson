using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterCorrelaçõesPossíveisConformeAsSelecionadas
{
    public interface IObterCorrelaçõesPossíveisConformeAsSelecionadasUseCase
    {
        Task<ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput> Execute(string idPoço,
            string grupoLitológicoSelecionado, string ucsSelecionada, string coesaSelecionada,
            string angatSelecionada, string restrSelecionada);
    }
}
