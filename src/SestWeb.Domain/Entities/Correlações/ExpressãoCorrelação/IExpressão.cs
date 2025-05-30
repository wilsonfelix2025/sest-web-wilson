using SestWeb.Domain.Entities.Correlações.ConstantesCorrelação;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.AnaliticsCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação;
using SestWeb.Domain.Entities.Correlações.VariáveisCorrelação;

namespace SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação
{
    public interface IExpressão 
    {
        string Bruta { get; }

        string Normalizada { get; }

        IVariáveis Variáveis { get; }

        IConstantes Constantes { get; }

        IPerfisEntrada PerfisEntrada { get; }

        IPerfisSaída PerfisSaída { get; }

        IAnalitics Analitics { get; }
    }
}
