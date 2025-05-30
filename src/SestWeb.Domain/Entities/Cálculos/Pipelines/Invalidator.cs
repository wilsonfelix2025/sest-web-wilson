using System;
using System.Collections.Generic;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Filtros;
using SestWeb.Domain.Entities.Cálculos.Pipelines.GerenciadorDePipelines;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Pipelines
{
    /// <summary>
    /// Classe responssável por zerar os perfis de saída dos cálculos.
    /// É chamada quando algum perfil , que é dependência direta ou indireta para o cálculo,é removido.
    /// </summary>
    public class Invalidator
    {
        private readonly IPipelineManager _pipelineManager;

        public Invalidator(List<ICálculo> cálculosDoPoço, List<IFiltro> filtrosDoPoço)
        {
            _pipelineManager = new PipelineManager(cálculosDoPoço, filtrosDoPoço);
        }

        public void Invalidate(PerfilBase perfil)
        {
            try
            {
                var cálculosDependentes = _pipelineManager.GetDependentCalculus(perfil);

                if (cálculosDependentes.Count == 0)
                    return;

                _pipelineManager.InvalidateDependentCalculus(perfil, cálculosDependentes);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Falha na invalidação : " + exception.Message);
            }
        }

        public void Invalidate(Trend.Trend trend, List<PerfilBase>perfisDoPoço)
        {
            try
            {
                _pipelineManager.InvalidateDependentCalculus(trend, perfisDoPoço);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Falha na invalidação : " + exception.Message);
            }
        }

        public void Invalidate(ILitologia litologia)
        {
            try
            {
                var cálculosDependentes = _pipelineManager.ObterCálculosDependentesDaLitologia(litologia);
                _pipelineManager.InvalidateDependentCalculus(cálculosDependentes);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Falha na invalidação : " + exception.Message);
            }
        }
    }
}
