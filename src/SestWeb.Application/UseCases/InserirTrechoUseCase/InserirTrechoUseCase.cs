using System;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.Validadores;
using SestWeb.Domain.DTOs.InserirTrecho;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Enums;
using SestWeb.Domain.InserirTrecho;

namespace SestWeb.Application.UseCases.InserirTrechoUseCase
{
    internal class InserirTrechoUseCase : IInserirTrechoUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;

        public InserirTrechoUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPerfilWriteOnlyRepository perfilWriteOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<InserirTrechoOutput> Execute(string idPerfil, InserirTrechoInput trechoInput)
        {
            try
            {
                var perfilSelecionado = await _perfilReadOnlyRepository.ObterPerfil(idPerfil);
                if (perfilSelecionado == null)
                    return InserirTrechoOutput.InserirTrechoComFalhaDeValidação("Perfil não encontrado.");

                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(perfilSelecionado.IdPoço);

                var validator = new InserirTrechoValidator(_perfilReadOnlyRepository,trechoInput, perfilSelecionado.IdPoço);
                var appResult = await validator.Validate();
                if (!string.IsNullOrEmpty(appResult))
                    return InserirTrechoOutput.InserirTrechoComFalhaDeValidação(string.Join("\n", appResult));

                var dto = PreencherDTO(trechoInput, perfilSelecionado, poço);

                var inserirTrecho = new InserirTrechos(poço.Trajetória, poço.ObterLitologiaPadrão(), dto);
                var result = inserirTrecho.InserirComplementoDeCurva();

                if (result.result.IsValid == false)
                    return InserirTrechoOutput.InserirTrechoComFalhaDeValidação(string.Join("\n", result.result.Errors));

                await _perfilWriteOnlyRepository.CriarPerfil(perfilSelecionado.IdPoço, (PerfilBase)result.Entity, poço);
                
                return InserirTrechoOutput.InserirTrechoCriadoComSucesso((PerfilBase) result.Entity);
                
            }
            catch (Exception e)
            {
                return InserirTrechoOutput.InserirTrechoComFalha(e.Message);
            }

        }

        private InserirTrechoDTO PreencherDTO(InserirTrechoInput input, PerfilBase perfilSelecionado, Poço poço)
        {
            var valorTopo = PreencherValorTopo(perfilSelecionado.Mnemonico, poço);

            var litologia = poço.TipoPoço == TipoPoço.Projeto ? 
                            poço.Litologias.Find(l => l.Classificação == TipoLitologia.Adaptada) :
                            poço.Litologias.Find(l => l.Classificação == TipoLitologia.Interpretada);


            var dto = new InserirTrechoDTO
            {
                LitologiasSelecionadas = input.LitologiasSelecionadas,
                BaseDeSedimentos = poço.ObterBaseDeSedimentos(),
                NovoNome = input.NomeNovoPerfil,
                PerfilSelecionado = perfilSelecionado,
                PmLimite = input.PMLimite,
                TipoDeTrecho = input.TipoDeTrecho,
                TipoTratamentoTrecho = input.TipoTratamento,
                ValorTopo = valorTopo,
                ÚltimoPontoTrajetória = poço.Trajetória.ÚltimoPonto.Pm.Valor,
                TrajetóriaPoço = poço.Trajetória,
                LitologiaPoço = litologia
            };

            return dto;
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
