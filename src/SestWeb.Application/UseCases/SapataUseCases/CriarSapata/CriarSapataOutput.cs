using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.SapataUseCases.CriarSapata
{
    public class CriarSapataOutput : UseCaseOutput<CriarSapataStatus>
    {
        private CriarSapataOutput()
        {
        }

        public static CriarSapataOutput SapataCriada()
        {
            return new CriarSapataOutput
            {
                Status = CriarSapataStatus.SapataCriada,
                Mensagem = "Sapata criada com sucesso."
            };
        }

        public static CriarSapataOutput SapataJáExiste(double profundidadeMedida)
        {
            return new CriarSapataOutput
            {
                Status = CriarSapataStatus.SapataNãoCriada,
                Mensagem = $"Já existe sapata na profundidade {profundidadeMedida}."
            };
        }

        public static CriarSapataOutput SapataNãoCriada()
        {
            return new CriarSapataOutput
            {
                Status = CriarSapataStatus.SapataNãoCriada,
                Mensagem = "Não foi possível criar a sapata."
            };
        }

        public static CriarSapataOutput SapataNãoCriada(string mensagem)
        {
            return new CriarSapataOutput
            {
                Status = CriarSapataStatus.SapataNãoCriada,
                Mensagem = $"Não foi possível criar a sapata. {mensagem}"
            };
        }
    }
}
