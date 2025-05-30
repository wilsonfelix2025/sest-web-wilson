using System;
using SestWeb.Domain.Entities.Correlações.ConstantesCorrelação;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.AnaliticsCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação;
using SestWeb.Domain.Entities.Correlações.VariáveisCorrelação;

namespace SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.Factory
{
    public class ExpressãoFactory : IExpressãoFactory
    {
        private static Func<string, IAnalitics, IVariáveis, IConstantes, IPerfisSaída, IPerfisEntrada, IExpressão> _ctorCaller;

        public ExpressãoFactory() { }

        public IExpressão CreateExpressão(string expressão)
        {
            Expressão.RegisterCorrelaçãoCtor();
            return _ctorCaller(expressão, new Analitics(expressão), Variáveis.GetFactory().CreateVariáveis(expressão),Constantes.GetFactory().CreateConstantes(expressão),PerfisSaída.GetFactory().CreatePerfisSaída(expressão),PerfisEntrada.GetFactory().CreatePerfisEntrada(expressão));
        }

        public static void RegisterExpressãoCtor(Func<string, IAnalitics, IVariáveis, IConstantes, IPerfisSaída, IPerfisEntrada, IExpressão> ctorCaller)
        {
            _ctorCaller = ctorCaller;
        }
    }
}
