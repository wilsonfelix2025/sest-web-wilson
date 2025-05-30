using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.LoadRelacionamentosCorrsPropMec
{
    public class LoadRelacionamentosCorrsPropMecOutput : UseCaseOutput<LoadRelacionamentosCorrsPropMecStatus>
    {
        private LoadRelacionamentosCorrsPropMecOutput() { }

        public IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico> Relacionamentos { get; private set; }

        public static LoadRelacionamentosCorrsPropMecOutput RelacionamentosCarregados(IReadOnlyCollection<RelacionamentoUcsCoesaAngatPorGrupoLitológico> relacionamentos)
        {
            return new LoadRelacionamentosCorrsPropMecOutput
            {
                Relacionamentos = relacionamentos,
                Status = LoadRelacionamentosCorrsPropMecStatus.RelacionamentosCarregados,
                Mensagem = "Relacionamentos carregados com sucesso."
            };
        }

        public static LoadRelacionamentosCorrsPropMecOutput RelacionamentosJáCarregados()
        {
            return new LoadRelacionamentosCorrsPropMecOutput
            {
                Status = LoadRelacionamentosCorrsPropMecStatus.RelacionamentosJáCarregados,
                Mensagem = $"Relacionamentos já carregados."
            };
        }

        public static LoadRelacionamentosCorrsPropMecOutput RelacionamentosNãoCarregados(string mensagem)
        {
            return new LoadRelacionamentosCorrsPropMecOutput
            {
                Status = LoadRelacionamentosCorrsPropMecStatus.RelacionamentosNãoCarregados,
                Mensagem = $"Não foi possível carregar os Relacionamentos. {mensagem}"
            };
        }

        public static LoadRelacionamentosCorrsPropMecOutput PoçoNãoEncontrado(string idPoço)
        {
            return new LoadRelacionamentosCorrsPropMecOutput
            {
                Status = LoadRelacionamentosCorrsPropMecStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }

        public static LoadRelacionamentosCorrsPropMecOutput CorrelaçõesNãoEncontradas()
        {
            return new LoadRelacionamentosCorrsPropMecOutput
            {
                Status = LoadRelacionamentosCorrsPropMecStatus.CorrelaçõesNãoEncontradas,
                Mensagem = $"Não foi possível encontrar correlações."
            };
        }

        public static LoadRelacionamentosCorrsPropMecOutput RelacionamentosNãoEncontrados()
        {
            return new LoadRelacionamentosCorrsPropMecOutput
            {
                Status = LoadRelacionamentosCorrsPropMecStatus.RelacionamentosNãoEncontrados,
                Mensagem = $"Não foi possível encontrar os relacionamentos."
            };
        }
    }
}
