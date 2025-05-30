using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.SapataUseCases.AtualizarSapatas
{
    public class AtualizarSapatasOutput : UseCaseOutput<AtualizarSapatasStatus>
    {
        public AtualizarSapatasOutput()
        {
            
        }

        public static AtualizarSapatasOutput SapatasAtualizadas()
        {
            return new AtualizarSapatasOutput
            {
                Status = AtualizarSapatasStatus.Sucesso,
                Mensagem = "Sapatas do poço atualizadas com sucesso."
            };
        }

        public static AtualizarSapatasOutput SapatasNãoAtualizadas(string mensagem = "")
        {
            return new AtualizarSapatasOutput
            {
                Status = AtualizarSapatasStatus.Falha,
                Mensagem = $"Não foi possível atualizar as sapatas do poço - {mensagem}"
            };
        }

        public static AtualizarSapatasOutput PoçoNãoEncontrado(string id)
        {
            return new AtualizarSapatasOutput
            {
                Status = AtualizarSapatasStatus.Falha,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }
    }
}
