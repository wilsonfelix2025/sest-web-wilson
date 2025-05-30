using SestWeb.Domain.Entities.PontosEntity;

namespace SestWeb.Domain.DTOs
{
    public class PontoDTO
    {
        public string Pm { get; set; }
        public string Valor { get; set; }
        public string Pv { get; set; }

        public OrigemPonto Origem { get; set; }

    }
}
