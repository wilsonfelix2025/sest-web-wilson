using SestWeb.Domain.Entities.Perfis.Base;
using System;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterTiposDePerfil
{
    internal class ObterTiposDePerfilUseCase : IObterTiposDePerfilUseCase
    {
        public ObterTiposDePerfilUseCase()
        {
        }

        public ObterTiposDePerfilOutput Execute()
        {
            try
            {
                var tiposDePerfil = TiposPerfil.Todos;//PerfilOld.GetTiposPerfil();
                
                return ObterTiposDePerfilOutput.TiposDePerfilObtidos(tiposDePerfil);
            }
            catch (Exception e)
            {
                return ObterTiposDePerfilOutput.TiposDePerfilNãoObtidos(e.Message);
            }
        }
    }
}