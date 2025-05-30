using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterPerfil
{
    public class ObterPerfilOutput : UseCaseOutput<ObterPerfilStatus>
    {
        private ObterPerfilOutput()
        {
        }

        public PerfilBase PerfilOld { get; private set; }

        public static ObterPerfilOutput PerfilObtido(PerfilBase perfilOld)
        {
            return new ObterPerfilOutput
            {
                PerfilOld = perfilOld,
                Status = ObterPerfilStatus.PerfilObtido,
                Mensagem = "Perfil obtido com sucesso."
            };
        }

        public static ObterPerfilOutput PerfilNãoEncontrado(string id)
        {
            return new ObterPerfilOutput
            {
                Status = ObterPerfilStatus.PerfilNãoEncontrado,
                Mensagem = $"Não foi possível encontrar perfil com id {id}."
            };
        }

        public static ObterPerfilOutput PerfilNãoObtido(string mensagem)
        {
            return new ObterPerfilOutput
            {
                Status = ObterPerfilStatus.PerfilNãoObtido,
                Mensagem = $"Não foi possível obter perfil. {mensagem}"
            };
        }
    }
}
