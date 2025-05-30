using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Application.UseCases.TrajetóriaUseCases.ObterTrajetória
{
    public class ObterTrajetóriaOutput : UseCaseOutput<ObterTrajetóriaStatus>
    {
        private ObterTrajetóriaOutput()
        {
        }

        public Trajetória Trajetória { get; private set; }

        public static ObterTrajetóriaOutput TrajetóriaObtida(Trajetória trajetóriaOld)
        {
            return new ObterTrajetóriaOutput
            {
                Status = ObterTrajetóriaStatus.TrajetóriaObtida,
                Mensagem = "Trajetória obtida com sucesso.",
                Trajetória = trajetóriaOld
            };
        }

        public static ObterTrajetóriaOutput PoçoNãoEncontrado(string id)
        {
            return new ObterTrajetóriaOutput
            {
                Status = ObterTrajetóriaStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }

        public static ObterTrajetóriaOutput TrajetóriaNãoObtida(string mensagem)
        {
            return new ObterTrajetóriaOutput
            {
                Status = ObterTrajetóriaStatus.TrajetóriaNãoObtida,
                Mensagem = $"Não foi possível obter trajetória. {mensagem}"
            };
        }
    }
}
