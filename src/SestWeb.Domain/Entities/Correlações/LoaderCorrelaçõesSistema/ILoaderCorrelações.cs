using System.Collections.Generic;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Domain.Entities.Correlações.LoaderCorrelaçõesSistema
{
    public interface ILoaderCorrelações
    {
        List<ICorrelação> Load();
    }
}
