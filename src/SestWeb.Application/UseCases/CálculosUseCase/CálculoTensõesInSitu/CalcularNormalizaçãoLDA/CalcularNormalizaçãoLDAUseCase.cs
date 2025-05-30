using SestWeb.Application.Repositories;
using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Base.SincroniaProfundidades;
using SestWeb.Domain.Entities.Cálculos.Tensões;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CalcularNormalizaçãoLDA
{
    public class CalcularNormalizaçãoLDAUseCase : ICalcularNormalizaçãoLDAUseCase
    {
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;

        public CalcularNormalizaçãoLDAUseCase(IPoçoReadOnlyRepository poçoReadOnlyRepository, IPerfilReadOnlyRepository perfilReadOnlyRepository)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
        }

        public async Task<CalcularNormalizaçãoLDAOutput> Execute(CalcularNormalizaçãoLDAInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return CalcularNormalizaçãoLDAOutput.CálculoNormalizaçãoLDANãoEfetuado("Poço não encontrado");

                var perfilTensãoVertical = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilTensãoVerticalId);

                var perfil = CalcularNormalizaçãoLDA(input, poço, perfilTensãoVertical);
                return CalcularNormalizaçãoLDAOutput.CálculoNormalizaçãoLDAEfetuado(perfil);

            }
            catch (Exception e)
            {
                return CalcularNormalizaçãoLDAOutput.CálculoNormalizaçãoLDANãoEfetuado(e.Message);
            }
        }

        private RetornoLotDTO CalcularNormalizaçãoLDA(CalcularNormalizaçãoLDAInput input, Poço poço, PerfilBase perfilEntrada)
        {
            var lot = PreencherParâmetrosLot(input);
            var sincronizadorProfundidades = new SincronizadorProfundidades(new List<PerfilBase> { perfilEntrada }, poço.Trajetória, poço.ObterLitologiaPadrão(), GrupoCálculo.Tensões);
            var profundidades = sincronizadorProfundidades.GetProfundidadeDeReferência();
            var calc = new CálculoTensãoHorizontalMenorNormalizaçãoLDA(perfilEntrada, lot, poço.Trajetória, poço.DadosGerais.Geometria, poço.DadosGerais, poço.ObterLitologiaPadrão(), input.Coeficiente);
            var retornoLot = calc.GerarDadosGráfico();

            return retornoLot;
        }

        private List<ParâmetrosLotDTO> PreencherParâmetrosLot(CalcularNormalizaçãoLDAInput input)
        {
            var list = new List<ParâmetrosLotDTO>();

            foreach (var item in input.ListaLot)
            {
                var param = new ParâmetrosLotDTO
                {
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
