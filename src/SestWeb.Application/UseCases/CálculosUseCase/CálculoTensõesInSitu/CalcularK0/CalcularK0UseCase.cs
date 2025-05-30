using SestWeb.Application.Repositories;
using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Cálculos.Tensões;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularK0
{
    public class CalcularK0UseCase : ICalcularK0UseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;

        public CalcularK0UseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPerfilReadOnlyRepository perfilReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
        }

        public async Task<CalcularK0Output> Execute(CalcularK0Input input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return CalcularK0Output.CálculoK0NãoEfetuado("Poço não encontrado");

                var perfilTensãoVertical = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilTensãoVerticalId);
                var perfilGPORO = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilGPOROId);

                var perfisEntrada = new List<PerfilBase>
                {
                    perfilTensãoVertical,
                    perfilGPORO
                };

                var perfil = CalcularK0(input, poço, perfisEntrada, true);
                return CalcularK0Output.CálculoK0Efetuado(perfil);

            }
            catch (Exception e)
            {
                return CalcularK0Output.CálculoK0NãoEfetuado(e.Message);
            }
        }

        private RetornoLotDTO CalcularK0(CalcularK0Input input, Poço poço, List<PerfilBase> entradas, bool retornaSomenteK0 = false)
        {
            var lot = PreencherParâmetrosLot(input);
            var calc = new CálculoK0(entradas, lot, poço.Trajetória, poço.DadosGerais.Geometria, poço.ObterLitologiaPadrão(), false);

            var resultadoK0 = calc.GerarDadosGráfico();

            return resultadoK0;
        }

        private List<ParâmetrosLotDTO> PreencherParâmetrosLot(CalcularK0Input input)
        {
            var list = new List<ParâmetrosLotDTO>();

            foreach (var item in input.ListaLot)
            {
                var param = new ParâmetrosLotDTO
                {
                    Lot = item.LOT.Value,
                    ProfundidadeVertical = item.PV.Value,
                    GradPressãoPoros = item.GradPressãoPoros,
                    Tvert = item.TVert
                };

                list.Add(param);
            }

            return list;
        }
    }
}
