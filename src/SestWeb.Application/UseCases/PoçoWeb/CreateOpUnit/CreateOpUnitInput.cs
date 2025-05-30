namespace SestWeb.Application.UseCases.PoçoWeb.CreateOpUnit
{
    public class CreateOpUnitInput
    {
        public CreateOpUnitInput(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
