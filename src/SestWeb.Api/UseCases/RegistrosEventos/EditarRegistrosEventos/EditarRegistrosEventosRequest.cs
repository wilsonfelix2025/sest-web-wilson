using SestWeb.Application.UseCases.RegistrosEventosUseCases.EditarRegistrosEventos;
using SestWeb.Domain.Enums;

namespace SestWeb.Api.UseCases.RegistrosEventos.EditarRegistrosEventos
{
    public class EditarRegistrosEventosRequest
    {
        public string IdPoço { get; set; }
        public TipoRegistroEvento Tipo { get; set; }

        public MarcadorRegistroEventoInput[] Marcadores { get; set; }

        public TrechoRegistroEventoInput[] Trechos { get; set; }
    }
}