namespace SestWeb.Application.UseCases.PoçoWeb.CreateOilField
{
    public class CreateOilFieldInput
    {
        public CreateOilFieldInput(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
