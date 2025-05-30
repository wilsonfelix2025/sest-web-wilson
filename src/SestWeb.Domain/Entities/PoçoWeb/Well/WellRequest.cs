namespace SestWeb.Domain.Entities.PoçoWeb.Well
{
    public class WellRequest
    {
        public WellRequest(string Name, string OilFieldUrl)
        {
            name = Name;
            oilfield = OilFieldUrl;
        }

        public string name { get; set; }
        public string oilfield { get; set; }
    }
}
