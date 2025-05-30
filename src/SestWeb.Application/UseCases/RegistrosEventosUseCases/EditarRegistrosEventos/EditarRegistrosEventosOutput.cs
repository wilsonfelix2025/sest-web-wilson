using SestWeb.Application.Helpers;
using System.Collections.Generic;
using SestWeb.Domain.Entities.RegistrosEventos;

namespace SestWeb.Application.UseCases.RegistrosEventosUseCases.EditarRegistrosEventos
{
    public class EditarRegistrosEventosOutput : UseCaseOutput<EditarRegistrosEventosStatus>
    {
        private EditarRegistrosEventosOutput()
        {
        }

        public List<RegistroEvento> RegistrosEventos { get; set; }

        public static EditarRegistrosEventosOutput RegistrosEventosEditados(List<RegistroEvento> registrosEventos)
        {
            return new EditarRegistrosEventosOutput
            {
                Status = EditarRegistrosEventosStatus.RegistrosEventosEditados,
                Mensagem = "Registros e eventos editados com sucesso.",
                RegistrosEventos = registrosEventos
            };
        }

        public static EditarRegistrosEventosOutput PoçoNãoEncontrado(string id)
        {
            return new EditarRegistrosEventosOutput
            {
                Status = EditarRegistrosEventosStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível editar os registros e eventos. Não foi possível encontrar poço com id {id}."
            };
        }

        public static EditarRegistrosEventosOutput RegistrosEventosNãoEditados(string mensagem)
        {
            return new EditarRegistrosEventosOutput
            {
                Status = EditarRegistrosEventosStatus.RegistrosEventosNãoEditados,
                Mensagem = $"Não foi possível editar os registros e eventos. {mensagem}"
            };
        }
    }
}
