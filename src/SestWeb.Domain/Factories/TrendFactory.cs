
using SestWeb.Domain.Entities.Perfis.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Validadores;
using SestWeb.Domain.Entities.Trend;

namespace SestWeb.Domain.Factories
{
    public sealed class TrendFactory
    {

        public TrendFactory()
        {
            
        }

        public Result CriarTrend(PerfilBase perfil, Poço poço)
        {
            var result = new Result();
            var tipoTrend = TipoTrendEnum.Compactação;

            var validator = new TrendValidator(poço, false);
            var validationResult = validator.Validate(perfil);

            result.result = validationResult;

            if (validationResult.IsValid == false)
                return result;

            if (perfil.PodeTerTrendCompactacao)
                tipoTrend = TipoTrendEnum.Compactação;
            else if (perfil.PodeTerTrendBaseFolhelho)
                tipoTrend = TipoTrendEnum.LBF;
            
            var trend = (Trend)Activator.CreateInstance(typeof(Trend), // TODO(Vanessa Chalub): Verificar possibilidade de remover a instanciação por reflection.  
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                new object[] { tipoTrend, perfil }, null);

            result.Entity = trend;

            return result;
        }

        public Result EditarTrend(PerfilBase perfilOld, TrendDTO trendDto, Poço poço)
        {
            var result = new Result();

            if (!string.IsNullOrWhiteSpace(trendDto.NomeTrend))
                perfilOld.Trend.SetNomeTrend(trendDto.NomeTrend);

            if (trendDto.Trechos != null)
                PreencherTrechos(perfilOld, trendDto.Trechos);

            var validator = new TrendValidator(poço, true);
            var validationResult = validator.Validate(perfilOld);

            result.result = validationResult;

            if (validationResult.IsValid == false)
                return result;

            result.Entity = perfilOld;

            return result;
        }

        private void PreencherTrechos(PerfilBase perfil, IEnumerable<TrechoTrendDTO> trechosDto)
        {
            var trechos = new List<TrechoTrend>();

            foreach (var dto in trechosDto)
            {
                TrechoTrend trecho;
                if (dto.Inclinação == null)
                    trecho = new TrechoTrend(dto.PvTopo, dto.ValorTopo, dto.PvBase, dto.ValorBase);
                else
                    trecho = new TrechoTrend(dto.PvTopo, dto.ValorTopo, dto.PvBase, dto.ValorBase, dto.Inclinação.Value);

                trechos.Add(trecho);
            }

            perfil.Trend.ResetTrechos(trechos);
        }
    }
}
