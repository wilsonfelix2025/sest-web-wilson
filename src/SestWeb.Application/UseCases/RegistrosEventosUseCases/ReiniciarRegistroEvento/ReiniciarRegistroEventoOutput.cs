using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.RegistrosEventosUseCases.ReiniciarRegistroEvento
{
    public class ReiniciarRegistroEventoOutput : UseCaseOutput<ReiniciarRegistroEventoStatus>
    {
        private ReiniciarRegistroEventoOutput()
        {
        }

        public static ReiniciarRegistroEventoOutput RegistroEventoReiniciado()
        {
            return new ReiniciarRegistroEventoOutput
            {
                Status = ReiniciarRegistroEventoStatus.RegistroEventoReiniciado,
                Mensagem = "Registro/evento reiniciado com sucesso."
            };
        }

        public static ReiniciarRegistroEventoOutput RegistroEventoNãoReiniciado(string mensagem)
        {
            return new ReiniciarRegistroEventoOutput
            {
                Status = ReiniciarRegistroEventoStatus.RegistroEventoNãoReiniciado,
                Mensagem = $"Não foi possível reiniciado o registro/evento. {mensagem}"
            };
        }

        public static ReiniciarRegistroEventoOutput RegistroEventoNãoEncontrado(string id)
        {
            return new ReiniciarRegistroEventoOutput
            {
                Status = ReiniciarRegistroEventoStatus.RegistroEventoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar registro/evento com id {id}."
            };
        }
    }
}