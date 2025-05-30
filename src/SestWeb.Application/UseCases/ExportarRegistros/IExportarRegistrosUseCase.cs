using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.ExportarRegistros
{
    public interface IExportarRegistrosUseCase
    {
        Task<ExportarRegistrosOutput> Execute(ExportarRegistrosInput input);
    }
}
