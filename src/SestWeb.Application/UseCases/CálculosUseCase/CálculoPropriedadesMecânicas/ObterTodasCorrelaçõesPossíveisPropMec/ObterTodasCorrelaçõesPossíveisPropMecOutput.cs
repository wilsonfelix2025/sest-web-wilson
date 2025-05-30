using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterTodasCorrelaçõesPossíveisPropMec
{

    public class ObterTodasCorrelaçõesPossíveisPropMecOutput : UseCaseOutput<ObterTodasCorrelaçõesPossíveisPropMecStatus>
    {
        private ObterTodasCorrelaçõesPossíveisPropMecOutput() { }

        public AllPropMecPossibleCorrs CorrelaçõesPossíveis { get; private set; }

        public static ObterTodasCorrelaçõesPossíveisPropMecOutput CorrelaçõesObtidas(AllPropMecPossibleCorrs correlaçõesPossíveis)
        {
            return new ObterTodasCorrelaçõesPossíveisPropMecOutput
            {
                CorrelaçõesPossíveis = correlaçõesPossíveis,
                Status = ObterTodasCorrelaçõesPossíveisPropMecStatus.CorrelaçõesObtidas,
                Mensagem = "Correlações obtidas com sucesso."
            };
        }

        public static ObterTodasCorrelaçõesPossíveisPropMecOutput CorrelaçõesNãoObtidas(string mensagem)
        {
            return new ObterTodasCorrelaçõesPossíveisPropMecOutput
            {
                Status = ObterTodasCorrelaçõesPossíveisPropMecStatus.CorrelaçõesNãoObtidas,
                Mensagem = $"Não foi possível obter as correlações. {mensagem}"
            };
        }

        public static ObterTodasCorrelaçõesPossíveisPropMecOutput PoçoNãoEncontrado(string idPoço)
        {
            return new ObterTodasCorrelaçõesPossíveisPropMecOutput
            {
                Status = ObterTodasCorrelaçõesPossíveisPropMecStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
