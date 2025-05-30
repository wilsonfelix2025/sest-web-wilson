using SestWeb.Domain.Entities.Perfis.Pontos;
using SestWeb.Domain.EstilosVisuais;
using System.Threading.Tasks;
using SestWeb.Domain.DTOs;

namespace SestWeb.Application.UseCases.PerfilUseCases.EditarPerfil
{
    public interface IEditarPerfilUseCase
    {
        Task<EditarPerfilOutput> Execute(string idPerfil, string nome, string descrição, EstiloVisual estiloVisual, PontoDTO[] pontosDTO, bool emPv);
    }
}