using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.PmPvUseCases
{
    public class PmPvOutput : UseCaseOutput<PmPvStatus>
    {
        private PmPvOutput()
        {
        }

        public static PmPvOutput ConversãoRealizada()
        {
            return new PmPvOutput
            {
                Status = PmPvStatus.ConversãoRealizada,
                Mensagem = "Conversão Pm/Pv realizada com sucesso."
            };
        }

        public static PmPvOutput ConversãoNãoRealizada(string mensagem)
        {
            return new PmPvOutput
            {
                Status = PmPvStatus.ConversãoNãoRealizada,
                Mensagem = $"Não foi possível realizar a conversão Pm/Pv. {mensagem}"
            };
        }
    }
}
