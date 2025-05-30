using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.ComposiçãoPerfil
{
    public interface IComporPerfilUseCase
    {
        Task<ComporPerfilOutput> Execute(ComporPerfilInput input, string idPoço);
    }
}
