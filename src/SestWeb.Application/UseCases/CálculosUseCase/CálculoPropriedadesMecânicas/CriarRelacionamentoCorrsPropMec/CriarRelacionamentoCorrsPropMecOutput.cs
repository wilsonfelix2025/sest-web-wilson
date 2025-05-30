using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarRelacionamentoCorrsPropMec
{
    public class CriarRelacionamentoCorrsPropMecOutput : UseCaseOutput<CriarRelacionamentoCorrsPropMecStatus>
    {
        private CriarRelacionamentoCorrsPropMecOutput() { }

        public RelacionamentoUcsCoesaAngatPorGrupoLitológico RelacionamentoPropMec { get; set; }

        public static CriarRelacionamentoCorrsPropMecOutput RelacionamentoCriado(RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamentoPropMec)
        {
            return new CriarRelacionamentoCorrsPropMecOutput
            {
                Status = CriarRelacionamentoCorrsPropMecStatus.RelacionamentoCriado,
                Mensagem = "Relacionamento criado com sucesso.",
                RelacionamentoPropMec = relacionamentoPropMec
            };
        }

        public static CriarRelacionamentoCorrsPropMecOutput RelacionamentoExistente(string grupoLito, string ucsCorr, string coesaCorr, string angatCorr, string restrCorr)
        {
            return new CriarRelacionamentoCorrsPropMecOutput
            {
                Status = CriarRelacionamentoCorrsPropMecStatus.RelacionamentoExistente,
                Mensagem = $"Já existe um relacionamento com as correlações ({ucsCorr}, {coesaCorr}, {angatCorr}, {restrCorr}) para o grupo litológico {grupoLito}."
            };
        }

        public static CriarRelacionamentoCorrsPropMecOutput RelacionamentoNãoCriado(string nome)
        {
            return new CriarRelacionamentoCorrsPropMecOutput
            {
                Status = CriarRelacionamentoCorrsPropMecStatus.RelacionamentoNãoCriado,
                Mensagem = $"Não foi possível criar relacionamento: {nome}"
            };
        }

        public static CriarRelacionamentoCorrsPropMecOutput PoçoNãoEncontrado(string idPoço)
        {
            return new CriarRelacionamentoCorrsPropMecOutput
            {
                Status = CriarRelacionamentoCorrsPropMecStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }

        public static CriarRelacionamentoCorrsPropMecOutput CorrelaçãoInexistente(string nome)
        {
            return new CriarRelacionamentoCorrsPropMecOutput
            {
                Status = CriarRelacionamentoCorrsPropMecStatus.CorrelaçãoInexistente,
                Mensagem = $"Não foi possível encontrar a correlação {nome}."
            };
        }

        public static CriarRelacionamentoCorrsPropMecOutput GrupoLitológicoNãoEncontrado(string grupoLito)
        {
            return new CriarRelacionamentoCorrsPropMecOutput
            {
                Status = CriarRelacionamentoCorrsPropMecStatus.GrupoLitológicoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar grupo litológico: {grupoLito}."
            };
        }
    }
}
