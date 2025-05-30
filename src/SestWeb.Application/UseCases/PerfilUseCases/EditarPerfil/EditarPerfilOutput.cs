using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Perfis.Base;
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.PerfilUseCases.EditarPerfil
{
    public class EditarPerfilOutput : UseCaseOutput<EditarPerfilStatus>
    {
        private EditarPerfilOutput()
        {
        }

        public PerfilBase Perfil { get; set; }
        public List<PerfilBase> PerfisAlterados { get; set; }

        public static EditarPerfilOutput PerfilEditado(PerfilBase perfil, List<PerfilBase> perfisAlterados)
        {
            return new EditarPerfilOutput
            {
                Status = EditarPerfilStatus.PerfilEditado,
                Mensagem = "Perfil editado com sucesso.",
                Perfil = perfil,
                PerfisAlterados = perfisAlterados
            };
        }

        public static EditarPerfilOutput PerfilNãoEncontrado(string id)
        {
            return new EditarPerfilOutput
            {
                Status = EditarPerfilStatus.PerfilNãoEditado,
                Mensagem = $"Não foi possível editar o perfil. Não foi possível encontrar perfil com id {id}."
            };
        }

        public static EditarPerfilOutput PerfilNãoEditado(string mensagem)
        {
            return new EditarPerfilOutput
            {
                Status = EditarPerfilStatus.PerfilNãoEditado,
                Mensagem = $"Não foi possível editar perfil. {mensagem}"
            };
        }
    }
}
