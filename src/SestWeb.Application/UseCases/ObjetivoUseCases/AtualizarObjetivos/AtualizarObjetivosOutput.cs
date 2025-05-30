using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.ObjetivoUseCases.AtualizarObjetivos
{
    public class AtualizarObjetivosOutput : UseCaseOutput<AtualizarObjetivosStatus>
    {
        public AtualizarObjetivosOutput()
        {

        }

        public static AtualizarObjetivosOutput ObjetivosAtualizados()
        {
            return new AtualizarObjetivosOutput
            {
                Status = AtualizarObjetivosStatus.Sucesso,
                Mensagem = "Objetivos do poço atualizados com sucesso."
            };
        }

        public static AtualizarObjetivosOutput ObjetivosNãoAtualizados(string mensagem = "")
        {
            return new AtualizarObjetivosOutput
            {
                Status = AtualizarObjetivosStatus.Falha,
                Mensagem = $"Não foi possível atualizar os objetivos do poço - {mensagem}"
            };
        }

        public static AtualizarObjetivosOutput PoçoNãoEncontrado(string id)
        {
            return new AtualizarObjetivosOutput
            {
                Status = AtualizarObjetivosStatus.Falha,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }
    }
}
