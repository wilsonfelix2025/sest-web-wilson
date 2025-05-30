using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.MontagemPerfis
{
    public interface IMontarPerfisUseCase
    {
        Task<MontarPerfisOutput> Execute(MontarPerfisInput input, string idPoço);
    }
}
