using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.LitologiaUseCases.CriarLitologia
{
    public class CriarLitologiaOutput : UseCaseOutput<CriarLitologiaStatus>
    {
        private CriarLitologiaOutput()
        {
        }

        public static CriarLitologiaOutput LitologiaCriada()
        {
            return new CriarLitologiaOutput
            {
                Status = CriarLitologiaStatus.LitologiaCriada,
                Mensagem = "Litologia criada com sucesso."
            };
        }

        public static CriarLitologiaOutput LitologiaNãoCriada(string mensagem = "")
        {
            return new CriarLitologiaOutput
            {
                Status = CriarLitologiaStatus.LitologiaNãoCriada,
                Mensagem = $"Não foi possível criar a litologia. {mensagem}"
            };
        }
    }
}
