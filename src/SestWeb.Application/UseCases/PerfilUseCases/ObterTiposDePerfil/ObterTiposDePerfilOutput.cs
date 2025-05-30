using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Perfis.Base;
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterTiposDePerfil
{
    public class ObterTiposDePerfilOutput : UseCaseOutput<ObterTiposDePerfilStatus>
    {
        public ObterTiposDePerfilOutput()
        {
        }

        public IReadOnlyCollection<TipoPerfil> TiposDePerfil { get; set; }

        public static ObterTiposDePerfilOutput TiposDePerfilObtidos(IReadOnlyCollection<TipoPerfil> tiposDePerfil)
        {
            return new ObterTiposDePerfilOutput
            {
                TiposDePerfil = tiposDePerfil,
                Status = ObterTiposDePerfilStatus.TiposDePerfilObtido,
                Mensagem = "Tipos de perfil obtidos com sucesso."
            };
        }

        public static ObterTiposDePerfilOutput TiposDePerfilNãoObtidos(string mensagem = "")
        {
            return new ObterTiposDePerfilOutput
            {
                Status = ObterTiposDePerfilStatus.TiposDePerfilNãoObtido,
                Mensagem = "Não foi possível obter os tipos de perfil - " + mensagem + "."
            };
        }
    }
}
