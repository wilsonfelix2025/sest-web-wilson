
namespace SestWeb.Domain.DTOs
{
    public class ObjetivoDTO
    {
        public ObjetivoDTO(string pm, string tipoObjetivo)
        {
            Pm = pm;
            TipoObjetivo = tipoObjetivo;
        }

        public string Pm { get; set; }
        public string TipoObjetivo { get; set; }
    }
}
