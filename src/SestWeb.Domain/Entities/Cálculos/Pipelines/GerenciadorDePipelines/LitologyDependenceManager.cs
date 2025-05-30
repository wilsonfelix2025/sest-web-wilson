using System.Collections.Generic;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Composto;
using SestWeb.Domain.Entities.Cálculos.Filtros;
using SestWeb.Domain.Entities.Cálculos.Perfis;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Cálculos.Pipelines.GerenciadorDePipelines
{
    public class LitologyDependenceManager
    {
        private readonly IPipelineManager _pipelineManager;

        public LitologyDependenceManager(IPipelineManager pipelineManager)
        {
            _pipelineManager = pipelineManager;
        }

        public List<ICálculo> GetDependentCalculus(ILitologia litologia)
        {
            List<ICálculo> cálculosDependentesDaLitologia = new List<ICálculo>();

            // pegar os cálculos de perfil e de sobrecarga
            var calcsDepDirLito = ObterCálculosDependentesDiretosDaLitologia(litologia);

            // pegar todos os dependentes desses cálculos
            foreach (var cálculo in calcsDepDirLito)
            {
                // adiciona o cálculo dependente 
                var nãoIncluído = cálculosDependentesDaLitologia.Find(c => c.Id == cálculo.Id) == null;
                if (nãoIncluído)
                    cálculosDependentesDaLitologia.Add(cálculo);

                // Adiciona os dependentes do cálculo dependentes
                var dependentes = _pipelineManager.GetDependentCalculus(cálculo);
                foreach (var dependente in dependentes)
                {
                    nãoIncluído = cálculosDependentesDaLitologia.Find(c => c.Id == dependente.Id) == null;

                    if (nãoIncluído)
                        cálculosDependentesDaLitologia.Add(dependente);
                }
            }

            //var cálculosDependentes = GetDependentCalculus(perfil, _cálculosDoPoço, _filtrosDoPoço);
            return _pipelineManager.OrdenadorCálculos.Order(cálculosDependentesDaLitologia);
        }

        private List<ICálculo> ObterCálculosDependentesDiretosDaLitologia(ILitologia lito)
        {
            List<ICálculo> calcDepDirLito = new List<ICálculo>();

            foreach (var calculo in _pipelineManager.CálculosDoPoço)
            {
                if (calculo.GrupoCálculo == GrupoCálculo.Perfis)
                {
                    var calculoComposto = (CálculoPerfis)calculo;
                    calcDepDirLito.Add(calculo);
                } else if (calculo.GrupoCálculo == GrupoCálculo.PropriedadesMecânicas)
                {
                    var calculoComposto = (CálculoPropriedadesMecânicas)calculo;
                    calcDepDirLito.Add(calculo);
                }
            }
            return calcDepDirLito;
        }

        public List<IFiltro> GetDependentFilters(ILitologia litologia)
        {
            List<IFiltro> filtrosDependentes = new List<IFiltro>();

            // pegar os cálculos de perfil e de sobrecarga
            var calcsDepDirLito = ObterCálculosDependentesDiretosDaLitologia(litologia);

            // pegar todos os filtros dependentes desses cálculos
            foreach (var cálculo in calcsDepDirLito)
            {
                var dependentes = _pipelineManager.GetDependentFilters(cálculo);
                foreach (var dependente in dependentes)
                {
                    var nãoIncluído = filtrosDependentes.Find(c => c.Id == dependente.Id) == null;

                    if (nãoIncluído)
                        filtrosDependentes.Add(dependente);
                }
            }
            return filtrosDependentes;
        }
    }
}
