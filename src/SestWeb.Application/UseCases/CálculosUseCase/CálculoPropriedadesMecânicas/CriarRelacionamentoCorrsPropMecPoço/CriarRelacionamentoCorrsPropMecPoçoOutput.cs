using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.PoçoRelacionamentoPropMec;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMecPoço
{
    public class CriarRelacionamentoCorrsPropMecPoçoOutput : UseCaseOutput<CriarRelacionamentoCorrsPropMecPoçoStatus>
    {
        private CriarRelacionamentoCorrsPropMecPoçoOutput() { }

        public RelacionamentoPropMecPoço RelacionamentoPropMecPoço { get; set; }

        public static CriarRelacionamentoCorrsPropMecPoçoOutput RelacionamentoCriado(RelacionamentoPropMecPoço relacionamentoPropMecPoço)
        {
            return new CriarRelacionamentoCorrsPropMecPoçoOutput
            {
                Status = CriarRelacionamentoCorrsPropMecPoçoStatus.RelacionamentoCriado,
                Mensagem = "Relacionamento criado com sucesso.",
                RelacionamentoPropMecPoço = relacionamentoPropMecPoço
            };
        }

        public static CriarRelacionamentoCorrsPropMecPoçoOutput RelacionamentoExistente(string grupoLito, string ucsCorr, string coesaCorr, string angatCorr, string restrCorr)
        {
            return new CriarRelacionamentoCorrsPropMecPoçoOutput
            {
                Status = CriarRelacionamentoCorrsPropMecPoçoStatus.RelacionamentoExistente,
                Mensagem = $"Já existe um relacionamento com as correlações ({ucsCorr}, {coesaCorr}, {angatCorr}, {restrCorr}) para o grupo litológico {grupoLito}."
            };
        }

        public static CriarRelacionamentoCorrsPropMecPoçoOutput RelacionamentoNãoCriado(string nome)
        {
            return new CriarRelacionamentoCorrsPropMecPoçoOutput
            {
                Status = CriarRelacionamentoCorrsPropMecPoçoStatus.RelacionamentoNãoCriado,
                Mensagem = $"Não foi possível criar relacionamento: {nome}"
            };
        }

        public static CriarRelacionamentoCorrsPropMecPoçoOutput PoçoNãoEncontrado(string idPoço)
        {
            return new CriarRelacionamentoCorrsPropMecPoçoOutput
            {
                Status = CriarRelacionamentoCorrsPropMecPoçoStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }

        public static CriarRelacionamentoCorrsPropMecPoçoOutput CorrelaçãoInexistente(string nome)
        {
            return new CriarRelacionamentoCorrsPropMecPoçoOutput
            {
                Status = CriarRelacionamentoCorrsPropMecPoçoStatus.CorrelaçãoInexistente,
                Mensagem = $"Não foi possível encontrar a correlação {nome}."
            };
        }

        public static CriarRelacionamentoCorrsPropMecPoçoOutput GrupoLitológicoNãoEncontrado(string grupoLito)
        {
            return new CriarRelacionamentoCorrsPropMecPoçoOutput
            {
                Status = CriarRelacionamentoCorrsPropMecPoçoStatus.GrupoLitológicoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar grupo litológico: {grupoLito}."
            };
        }
    }
}
