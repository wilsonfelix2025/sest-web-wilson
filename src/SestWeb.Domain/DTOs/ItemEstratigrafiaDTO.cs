namespace SestWeb.Domain.DTOs
{
    public class ItemEstratigrafiaDTO
    {
        public string Pm { get; }

        public ItemEstratigrafiaDTO(string pm, string sigla, string descrição, string idade, string basePm = "")
        {
            Pm = pm;
            Sigla = sigla;
            Descrição = descrição;
            Idade = idade;
            BasePm = basePm;
        }

        public string Sigla { get; set; }
        public string Descrição { get; set; }
        public string Idade { get; set; }
        public string BasePm { get; set; } = string.Empty;
    }
}
