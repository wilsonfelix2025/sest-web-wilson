using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.PerfilUseCases.CriarPerfil
{
    public class CriarPerfilOutput : UseCaseOutput<CriarPerfilStatus>
    {
        private CriarPerfilOutput()
        {
        }

        public PerfilBase PerfilOld { get; set; }

        public static CriarPerfilOutput PerfilCriado(PerfilBase perfilOld)
        {
            return new CriarPerfilOutput
            {
                Status = CriarPerfilStatus.PerfilCriado,
                Mensagem = "Perfil criado com sucesso.",
                PerfilOld = perfilOld
            };
        }

        public static CriarPerfilOutput PoçoNãoEncontrado(string id)
        {
            return new CriarPerfilOutput
            {
                Status = CriarPerfilStatus.PerfilNãoCriado,
                Mensagem = $"Não foi possível criar perfil. Não foi possível encontrar poço com id {id}."
            };
        }

        public static CriarPerfilOutput PerfilNãoCriado(string mensagem)
        {
            return new CriarPerfilOutput
            {
                Status = CriarPerfilStatus.PerfilNãoCriado,
                Mensagem = $"Não foi possível criar perfil. {mensagem}"
            };
        }
    }
}
