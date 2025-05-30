using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarDadosGerais
{
    public class AtualizarDadosGeraisOutput : UseCaseOutput<AtualizarDadosGeraisStatus>
    {
        private AtualizarDadosGeraisOutput()
        {
        }

        public static AtualizarDadosGeraisOutput DadosGeraisAtualizados()
        {
            return new AtualizarDadosGeraisOutput
            {
                Status = AtualizarDadosGeraisStatus.DadosGeraisAtualizados,
                Mensagem = "Dados gerais atualizados com sucesso."
            };
        }

        public static AtualizarDadosGeraisOutput PoçoNãoEncontrado(string id)
        {
            return new AtualizarDadosGeraisOutput
            {
                Status = AtualizarDadosGeraisStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }

        public static AtualizarDadosGeraisOutput DadosGeraisNãoAtualizados(string mensagem = "")
        {
            return new AtualizarDadosGeraisOutput
            {
                Status = AtualizarDadosGeraisStatus.DadosGeraisNãoAtualizados,
                Mensagem = $"Não foi possível atualizar dados gerais. {mensagem}"
            };
        }
    }
}