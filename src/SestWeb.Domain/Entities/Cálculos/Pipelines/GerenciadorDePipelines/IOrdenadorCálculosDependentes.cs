using System.Collections.Generic;
using SestWeb.Domain.Entities.Cálculos.Base;

namespace SestWeb.Domain.Entities.Cálculos.Pipelines.GerenciadorDePipelines
{
    public interface IOrdenadorCálculosDependentes
    {
        List<ICálculo> Order(List<ICálculo> cálculosDependentes);
    }
}