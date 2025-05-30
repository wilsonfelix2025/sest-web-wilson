using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Correlações.VariablesCorrelação
{
    public interface IVariables
    {
        List<string> Names { get; }
        bool Exists(string variable);
        bool Any();
        bool TryGetValue(string name, out double valor);
    }
}
