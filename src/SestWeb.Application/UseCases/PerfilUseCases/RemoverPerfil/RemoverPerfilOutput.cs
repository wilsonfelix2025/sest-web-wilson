using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.PerfilUseCases.RemoverPerfil
{
    public class RemoverPerfilOutput : UseCaseOutput<RemoverPerfilStatus>
    {
        private RemoverPerfilOutput()
        {
        }

        public static RemoverPerfilOutput PerfilRemovido()
        {
            return new RemoverPerfilOutput
            {
                Status = RemoverPerfilStatus.PerfilRemovido,
                Mensagem = "Perfil removido com sucesso."
            };
        }

        public static RemoverPerfilOutput PerfilNãoRemovido(string mensagem)
        {
            return new RemoverPerfilOutput
            {
                Status = RemoverPerfilStatus.PerfilNãoRemovido,
                Mensagem = $"Não foi possível remover o perfil. {mensagem}"
            };
        }

        public static RemoverPerfilOutput PerfilNãoEncontrado(string id)
        {
            return new RemoverPerfilOutput
            {
                Status = RemoverPerfilStatus.PerfilNãoEncontrado,
                Mensagem = $"Não foi possível encontrar perfil com id {id}."
            };
        }
    }
}