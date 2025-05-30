using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;

namespace SestWeb.Application.UseCases.PoçoUseCases.ObterDadosGerais
{
    public class ObterDadosGeraisOutput : UseCaseOutput<ObterDadosGeraisStatus>
    {
        private ObterDadosGeraisOutput()
        {
        }

        public DadosGerais DadosGerais { get; private set; }

        public static ObterDadosGeraisOutput DadosGeraisObtidos(DadosGerais dadosGerais)
        {
            return new ObterDadosGeraisOutput
            {
                Status = ObterDadosGeraisStatus.DadosGeraisObtidos,
                Mensagem = "Dados gerais obtidos com sucesso.",
                DadosGerais = dadosGerais
            };
        }

        public static ObterDadosGeraisOutput PoçoNãoEncontrado(string id)
        {
            return new ObterDadosGeraisOutput
            {
                Status = ObterDadosGeraisStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }

        public static ObterDadosGeraisOutput DadosGeraisNãoObtidos(string mensagem = "")
        {
            return new ObterDadosGeraisOutput
            {
                Status = ObterDadosGeraisStatus.DadosGeraisNãoObtidos,
                Mensagem = $"Não foi possível obter dados gerais. {mensagem}"
            };
        }
    }
}