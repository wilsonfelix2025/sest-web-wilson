namespace SestWeb.Domain.DTOs
{
    public struct SapataDTO
    {
        public SapataDTO(string pm, string diâmetro)
        {
            Pm = pm;
            Diâmetro = diâmetro;
        }
        public string Pm { get; set; }
        public string Diâmetro { get; set; }
    }
}
