using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterCorrelaçõesPossíveisConformeAsSelecionadas
{
    public class ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput : UseCaseOutput<ObterCorrelaçõesPossíveisConformeAsSelecionadasStatus>
    {
        private ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput() { }

        public PropMecPossibleCorrsPerLitoGroup CorrelaçõesPossíveis { get; private set; }

        public static ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput CorrelaçõesObtidas(PropMecPossibleCorrsPerLitoGroup correlaçõesPossíveis)
        {
            return new ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput
            {
                CorrelaçõesPossíveis = correlaçõesPossíveis,
                Status = ObterCorrelaçõesPossíveisConformeAsSelecionadasStatus.CorrelaçõesObtidas,
                Mensagem = "Correlações obtidas com sucesso."
            };
        }

        public static ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput CorrelaçõesNãoObtidas(string mensagem)
        {
            return new ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput
            {
                Status = ObterCorrelaçõesPossíveisConformeAsSelecionadasStatus.CorrelaçõesNãoObtidas,
                Mensagem = $"Não foi possível obter as correlações. {mensagem}"
            };
        }

        public static ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput CorrelaçãoNãoEncontrada(string nome)
        {
            return new ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput
            {
                Status = ObterCorrelaçõesPossíveisConformeAsSelecionadasStatus.CorrelaçãoNãoEncontrada,
                Mensagem = $"Não foi possível encontrar a correlação: {nome}."
            };
        }

        public static ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput PoçoNãoEncontrado(string idPoço)
        {
            return new ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput
            {
                Status = ObterCorrelaçõesPossíveisConformeAsSelecionadasStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }

        public static ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput GrupoLitológicoNãoEncontrado(string grupoLito)
        {
            return new ObterCorrelaçõesPossíveisConformeAsSelecionadasOutput
            {
                Status = ObterCorrelaçõesPossíveisConformeAsSelecionadasStatus.GrupoLitológicoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar grupo litológico: {grupoLito}."
            };
        } 
    }
}
