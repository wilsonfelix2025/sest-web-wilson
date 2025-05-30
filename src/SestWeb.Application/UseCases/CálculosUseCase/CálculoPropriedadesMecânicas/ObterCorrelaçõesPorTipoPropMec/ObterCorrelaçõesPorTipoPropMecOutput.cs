using System.Collections.Generic;
using System.Linq;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Dto;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.ObterCorrelaçõesPorTipoPropMec
{
    public class ObterCorrelaçõesPorTipoPropMecOutput : UseCaseOutput<ObterCorrelaçõesPorTipoPropMecStatus>
    {
        private ObterCorrelaçõesPorTipoPropMecOutput() { }

        public List<CorrelaçãoDto> Correlações { get; private set; }

        public static ObterCorrelaçõesPorTipoPropMecOutput CorrelaçõesObtidas(List<Correlação> correlações)
        {
            return new ObterCorrelaçõesPorTipoPropMecOutput
            {
                Correlações = correlações.Select(corr => new CorrelaçãoDto(corr.Nome, corr.PerfisSaída.Tipos[0], corr.PerfisEntrada.Tipos, corr.Autor.Chave, corr.Origem.ToString())).ToList(),
                Status = ObterCorrelaçõesPorTipoPropMecStatus.CorrelaçõesObtidas,
                Mensagem = "Correlações obtidas com sucesso."
            };
        }

        public static ObterCorrelaçõesPorTipoPropMecOutput CorrelaçõesNãoObtidas(string mensagem)
        {
            return new ObterCorrelaçõesPorTipoPropMecOutput
            {
                Status = ObterCorrelaçõesPorTipoPropMecStatus.CorrelaçõesNãoObtidas,
                Mensagem = $"Não foi possível obter as correlações. {mensagem}"
            };
        }

        public static ObterCorrelaçõesPorTipoPropMecOutput PoçoNãoEncontrado(string idPoço)
        {
            return new ObterCorrelaçõesPorTipoPropMecOutput
            {
                Status = ObterCorrelaçõesPorTipoPropMecStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {idPoço}."
            };
        }

        public static ObterCorrelaçõesPorTipoPropMecOutput GrupoLitológicoNãoEncontrado(string grupoLito)
        {
            return new ObterCorrelaçõesPorTipoPropMecOutput
            {
                Status = ObterCorrelaçõesPorTipoPropMecStatus.GrupoLitológicoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar grupo litológico: {grupoLito}."
            };
        }
    }
}
