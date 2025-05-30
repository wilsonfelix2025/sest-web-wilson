using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação;

namespace SestWeb.Domain.Entities.Correlações.Base
{
    public interface ICorrelação
    {
        #region Properties

        string Nome { get; }

        IAutor Autor { get; }

        string Descrição { get; }

        Origem Origem { get; }

        IExpressão Expressão { get; }

        IPerfisEntrada PerfisEntrada { get; }

        IPerfisSaída PerfisSaída { get; }

        #endregion

    }
}
