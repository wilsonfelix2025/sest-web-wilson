using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.ComposiçãoPerfil
{
    public class ComporPerfilOutput : UseCaseOutput<ComporPerfilStatus>
    {
        public ComporPerfilOutput()
        {

        }


        public PerfilBase Perfil;

        public static ComporPerfilOutput ComporPerfilComSucesso(PerfilBase perfil)
        {
            return new ComporPerfilOutput
            {
                Status = ComporPerfilStatus.ComporPerfilComSucesso,
                Mensagem = "Composição de perfil com sucesso.",
                Perfil = perfil,
            };
        }

        public static ComporPerfilOutput ComporPerfilComFalhaDeValidação(string mensagem)
        {
            return new ComporPerfilOutput
            {
                Status = ComporPerfilStatus.ComporPerfilComFalhaDeValidação,
                Mensagem = $"Não foi possível compor perfil. {mensagem}"
            };
        }

        public static ComporPerfilOutput ComporPerfilComFalha(string mensagem)
        {
            return new ComporPerfilOutput
            {
                Status = ComporPerfilStatus.ComporPerfilComFalha,
                Mensagem = $"Não foi possível compor perfil. {mensagem}"
            };
        }

    }
}
