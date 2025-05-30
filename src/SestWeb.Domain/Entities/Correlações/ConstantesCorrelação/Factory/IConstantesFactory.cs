using SestWeb.Domain.Entities.Correlações.VariablesCorrelação;

namespace SestWeb.Domain.Entities.Correlações.ConstantesCorrelação.Factory
{
    public interface IConstantesFactory : IVariablesFactory
    {
        IConstantes CreateConstantes(string expressão);
    }
}
