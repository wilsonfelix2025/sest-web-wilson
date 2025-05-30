using SestWeb.Application.Repositories;
using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Cálculos.Tensões;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularNormalizaçãoPP
{
    public class CalcularNormalizaçãoPPUseCase : ICalcularNormalizaçãoPPUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;

        public CalcularNormalizaçãoPPUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPerfilReadOnlyRepository perfilReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
        }
        

        public async Task<CalcularNormalizaçãoPPOutput> Execute(CalcularNormalizaçãoPPInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return CalcularNormalizaçãoPPOutput.CálculoNormalizaçãoPPNãoEfetuado("Poço não encontrado");

                var perfilTensãoVertical = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilTensãoVerticalId);
                var perfilGPORO = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilGPOROId);

                var perfisEntrada = new List<PerfilBase>
                {
                    perfilTensãoVertical,
                    perfilGPORO
                };

                var retorno = CalcularNormalizaçãoPP(input, poço, perfisEntrada);
                return CalcularNormalizaçãoPPOutput.CálculoNormalizaçãoPPEfetuado(retorno);

            }
            catch (Exception e)
            {
                return CalcularNormalizaçãoPPOutput.CálculoNormalizaçãoPPNãoEfetuado(e.Message);
            }
        }

        private RetornoLotDTO CalcularNormalizaçãoPP(CalcularNormalizaçãoPPInput input, Poço poço, List<PerfilBase> perfisEntrada)
        {
            var lot = PreencherParâmetrosLot(input);
            var calc = new CálculoTensãoHorizontalMenorNormalizaçãoPressãoPoros(perfisEntrada, lot, poço.Trajetória, poço.DadosGerais.Geometria, poço.ObterLitologiaPadrão(), input.Coeficiente);
            var retorno = calc.GerarDadosGráfico();

            return retorno;
        }

        private List<ParâmetrosLotDTO> PreencherParâmetrosLot(CalcularNormalizaçãoPPInput input)
        {
            var list = new List<ParâmetrosLotDTO>();

            foreach (var item in input.ListaLot)
            {
                var param = new ParâmetrosLotDTO
                {
                    GradPressãoPoros = item.GradPressãoPoros.Value,
                    Lda = item.LDA.Value,
                    Lot = item.LOT.Value,
                    MesaRotativa = item.MR.Value,
                    ProfundidadeVertical = item.PV.Value
                };

                list.Add(param);
            }

            return list;
        }
    }
}
