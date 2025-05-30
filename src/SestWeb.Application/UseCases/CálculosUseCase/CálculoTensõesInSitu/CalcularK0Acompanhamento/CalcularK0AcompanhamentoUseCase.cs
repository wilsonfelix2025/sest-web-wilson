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
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularK0Acompanhamento
{
    public class CalcularK0AcompanhamentoUseCase : ICalcularK0AcompanhamentoUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;

        public CalcularK0AcompanhamentoUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPerfilReadOnlyRepository perfilReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
        }

        public async Task<CalcularK0AcompanhamentoOutput> Execute(CalcularK0AcompanhamentoInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return CalcularK0AcompanhamentoOutput.CálculoK0AcompanhamentoNãoEfetuado("Poço não encontrado");

                var perfilTensãoVertical = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilTensãoVerticalId);
                var perfilGPORO = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilGPOROId);

                var perfisEntrada = new List<PerfilBase>
                {
                    perfilTensãoVertical,
                    perfilGPORO
                };

                var perfil = CalcularK0Acompanhamento(input, poço, perfisEntrada, true);
                return CalcularK0AcompanhamentoOutput.CálculoK0AcompanhamentoEfetuado(perfil);

            }
            catch (Exception e)
            {
                return CalcularK0AcompanhamentoOutput.CálculoK0AcompanhamentoNãoEfetuado(e.Message);
            }
        }

        private PerfilBase CalcularK0Acompanhamento(CalcularK0AcompanhamentoInput input, Poço poço, List<PerfilBase> entradas, bool retornaSomenteK0 = false)
        {
            var lot = PreencherParâmetrosLot(input);
            var calc = new CálculoK0(entradas, lot, poço.Trajetória, poço.DadosGerais.Geometria, poço.ObterLitologiaPadrão(), true);

            var resultadoK0 = calc.Calcular();

            var k0Perfil = PerfisFactory.Create("K0", "K0", poço.Trajetória, poço.ObterLitologiaPadrão());

            foreach (var item in resultadoK0)
            {
                k0Perfil.AddPontoEmPm(poço.Trajetória, item.Item1, item.Item2, TipoProfundidade.PM, OrigemPonto.Calculado);
            }

            if (retornaSomenteK0)
                return k0Perfil;

            var perfisEntrada = new List<PerfilBase>(entradas);
            perfisEntrada.Add(k0Perfil);

            //calcular tensão horizontal menor, modelo K0
            var calculo = new CálculoTensãoHorizontalMenorK0(perfisEntrada, poço.Trajetória, poço.DadosGerais.Geometria, poço.ObterLitologiaPadrão());
            var resultado = calculo.Calcular();

            var perfil = PerfisFactory.Create("THORmin", "THORMin", poço.Trajetória, poço.ObterLitologiaPadrão());

            foreach (var item in resultado)
            {
                perfil.AddPontoEmPm(poço.Trajetória, item.Item1, item.Item2, TipoProfundidade.PM, OrigemPonto.Calculado);
            }


            return perfil;
        }

        private List<ParâmetrosLotDTO> PreencherParâmetrosLot(CalcularK0AcompanhamentoInput input)
        {
            var list = new List<ParâmetrosLotDTO>();

            foreach (var item in input.ListaLot)
            {
                var param = new ParâmetrosLotDTO
                {
                    Lot = item.LOT.Value,
                    ProfundidadeVertical = item.PV.Value
                };

                list.Add(param);
            }

            return list;
        }
    }
}
