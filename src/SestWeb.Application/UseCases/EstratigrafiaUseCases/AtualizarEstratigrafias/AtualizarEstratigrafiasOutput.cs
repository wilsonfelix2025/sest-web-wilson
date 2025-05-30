using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.AtualizarEstratigrafias
{
    public class AtualizarEstratigrafiasOutput : UseCaseOutput<AtualizarEstratigrafiasStatus>
    {
        private AtualizarEstratigrafiasOutput()
        {
        }

        public static AtualizarEstratigrafiasOutput EstratigrafiasAtualizadas()
        {
            return new AtualizarEstratigrafiasOutput
            {
                Status = AtualizarEstratigrafiasStatus.Sucesso,
                Mensagem = "Estratigrafias do poço atualizadas com sucesso."
            };
        }

        public static AtualizarEstratigrafiasOutput EstratigrafiasNãoAtualizadas(string mensagem = "")
        {
            return new AtualizarEstratigrafiasOutput
            {
                Status = AtualizarEstratigrafiasStatus.Falha,
                Mensagem = $"Não foi possível atualizar as estratigrafias do poço - {mensagem}"
            };
        }

        public static AtualizarEstratigrafiasOutput PoçoNãoEncontrado(string id)
        {
            return new AtualizarEstratigrafiasOutput
            {
                Status = AtualizarEstratigrafiasStatus.Falha,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }
    }
}
