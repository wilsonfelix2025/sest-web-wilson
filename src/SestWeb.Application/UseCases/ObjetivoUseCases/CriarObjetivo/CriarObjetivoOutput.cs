using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.ObjetivoUseCases.CriarObjetivo
{
    public class CriarObjetivoOutput : UseCaseOutput<CriarObjetivoStatus>
    {
        private CriarObjetivoOutput()
        {
        }

        public static CriarObjetivoOutput ObjetivoCriado()
        {
            return new CriarObjetivoOutput
            {
                Status = CriarObjetivoStatus.ObjetivoCriado,
                Mensagem = "Objetivo criado com sucesso."
            };
        }

        public static CriarObjetivoOutput ObjetivoNãoCriado(string mensagem = "")
        {
            return new CriarObjetivoOutput
            {
                Status = CriarObjetivoStatus.ObjetivoNãoCriado,
                Mensagem = $"Não foi possível criar o objetivo. {mensagem}"
            };
        }

        public static CriarObjetivoOutput ObjetivoJáExiste(double profundidadeMedida)
        {
            return new CriarObjetivoOutput
            {
                Status = CriarObjetivoStatus.ObjetivoNãoCriado,
                Mensagem = $"Já existe objetivo na profundidade {profundidadeMedida}."
            };
        }
    }
}
