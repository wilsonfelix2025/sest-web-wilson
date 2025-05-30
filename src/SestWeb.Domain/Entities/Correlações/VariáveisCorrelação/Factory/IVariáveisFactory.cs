using SestWeb.Domain.Entities.Correlações.VariablesCorrelação;

namespace SestWeb.Domain.Entities.Correlações.VariáveisCorrelação.Factory
{
    public interface IVariáveisFactory : IVariablesFactory
    {
        IVariáveis CreateVariáveis(string expressão);
    }
}
