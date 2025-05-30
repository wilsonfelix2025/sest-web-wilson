using SestWeb.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisParaTrecho
{
    internal class ObterPerfisTrechoUseCase : IObterPerfisTrechoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;

        public ObterPerfisTrechoUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPerfilReadOnlyRepository perfilReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
        }

        public async Task<ObterPerfisTrechoOutput> Execute(string idPoço)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(idPoço);

                if (poço == null)
                    return ObterPerfisTrechoOutput.PerfisNãoObtidos($"Não foi possível encontrar poço com id {idPoço}.");

                var perfis = await _perfilReadOnlyRepository.ObterPerfisTrechoDeUmPoço(idPoço);

                perfis = perfis.Where(p => p.PodeSerUsadoParaComplementarTrecho).ToList();

                var model = PreencherModel(poço, perfis);

                return ObterPerfisTrechoOutput.PerfisObtidos(model);
            }
            catch (Exception e)
            {
                return ObterPerfisTrechoOutput.PerfisNãoObtidos(e.Message);
            }
        }

        private ObterPerfisTrechoModel PreencherModel(Poço poço, IReadOnlyCollection<PerfilBase> perfis)
        {
            var model = new ObterPerfisTrechoModel
            {
                PMBase = poço.Trajetória.ÚltimoPonto.Pm.Valor,
                PMTopo = poço.ObterBaseDeSedimentos(),
                ListaPerfis = new List<PerfilModel>()
            };

            foreach (var perfil in perfis)
            {
                model.ListaPerfis.Add(new PerfilModel
                {
                    PMFinalPerfil = perfil.UltimoPonto.Pm.Valor,
                    PMInicialPerfil = perfil.PrimeiroPonto.Pm.Valor,
                    ValorTopoPerfil = PreencherValorTopo(perfil.Mnemonico, poço),
                    Id = perfil.Id.ToString(),
                    Mnemônico = perfil.Mnemonico,
                    Nome = perfil.Nome,
                    Unidade = perfil.GrupoDeUnidades.UnidadePadrão.Símbolo
                    
                });
            }

            return model;
        }

        private double PreencherValorTopo(string mnemonico, Poço poço)
        {
            var valor = 0.0;

            if (mnemonico == "DTC")
                valor = poço.DadosGerais.Area.SonicoSuperficie;
            else if (mnemonico == "RHOB")
                valor = poço.DadosGerais.Area.DensidadeSuperficie;
            else if (mnemonico == "DTS")
                valor = poço.DadosGerais.Area.DTSSuperficie;

            return valor;
        }
    }
}
