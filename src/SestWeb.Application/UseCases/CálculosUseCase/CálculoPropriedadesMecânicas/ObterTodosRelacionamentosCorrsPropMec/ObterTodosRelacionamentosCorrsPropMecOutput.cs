using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterTodosRelacionamentosCorrsPropMec
{
    public class ObterTodosRelacionamentosCorrsPropMecOutput : UseCaseOutput<ObterTodosRelacionamentosCorrsPropMecStatus>
    {
        private ObterTodosRelacionamentosCorrsPropMecOutput() { }

        public IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico> Relacionamentos { get; private set; }

        public static ObterTodosRelacionamentosCorrsPropMecOutput RelacionamentosObtidos(IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico> relacionamentos)
        {
            return new ObterTodosRelacionamentosCorrsPropMecOutput
            {
                Relacionamentos = relacionamentos,
                Status = ObterTodosRelacionamentosCorrsPropMecStatus.RelacionamentosObtidos,
                Mensagem = "Relacionamentos obtidos com sucesso."
            };
        }

        public static ObterTodosRelacionamentosCorrsPropMecOutput RelacionamentosNãoObtidos(string mensagem)
        {
            return new ObterTodosRelacionamentosCorrsPropMecOutput
            {
                Status = ObterTodosRelacionamentosCorrsPropMecStatus.RelacionamentosNãoObtidos,
                Mensagem = $"Não foi possível obter os relacionamentos. {mensagem}"
            };
        }

        public static ObterTodosRelacionamentosCorrsPropMecOutput RelacionamentosNãoEncontrados()
        {
            return new ObterTodosRelacionamentosCorrsPropMecOutput
            {
                Status = ObterTodosRelacionamentosCorrsPropMecStatus.RelacionamentosNãoEncontrados,
                Mensagem = $"Não foi possível encontrar relacionamentos."
            };
        }

        public static ObterTodosRelacionamentosCorrsPropMecOutput PoçoNãoEncontrado(string idPoço)
        {
            return new ObterTodosRelacionamentosCorrsPropMecOutput
            {
                Status = ObterTodosRelacionamentosCorrsPropMecStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
