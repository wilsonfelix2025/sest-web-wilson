using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Importadores.Shallow.Utils;

namespace SestWeb.Application.UseCases.LitologiaUseCases.EditarLitologia
{
    internal class EditarLitologiaUseCase : IEditarLitologiaUseCase
    {
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPipelineUseCase _pipeUseCase;


        public EditarLitologiaUseCase(IPoçoWriteOnlyRepository poçoWriteOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository, IPipelineUseCase pipelineUseCase)
        {
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _pipeUseCase = pipelineUseCase;
        }

        public async Task<EditarLitologiaOutput> Execute(string idPoço, string tipoLitologia, Dictionary<string, string>[] pontos)
        {
            try
            {
                var existePoço = await _poçoReadOnlyRepository.ExistePoço(idPoço);
                if (!existePoço)
                {
                    return EditarLitologiaOutput.PoçoNãoEncontrado(idPoço);
                }

                var poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);
                var litologia = ObterLitologiaEmPoço(poço, tipoLitologia);

                if (litologia == null)
                {
                    return EditarLitologiaOutput.LitologiaNãoEncontrada(tipoLitologia);
                }

                var pontosLitologia = new List<PontoLitologia>();
                litologia.Clear();

                foreach(var ponto in pontos)
                {
                    var éNumeroValido = int.TryParse(ponto["codigo"], out var codigoRocha);
                    if (!éNumeroValido)
                    {
                        return EditarLitologiaOutput.LitologiaNãoEditada($"Foi recebido um código em formato incorreto: '{ponto["codigo"]}'");
                    }

                    var tipoRocha = TipoRocha.FromNumero(codigoRocha);
                    if (tipoRocha == null)
                    {
                        return EditarLitologiaOutput.LitologiaNãoEditada($"Nenhuma litologia com código {codigoRocha} foi encontrada.");
                    }

                    double pmPonto = StringUtils.ToDoubleInvariantCulture(ponto["pm"], 2);

                    litologia.AddPontoEmPm(poço.Trajetória, new Profundidade(pmPonto), tipoRocha.Mnemonico, TipoProfundidade.PM, OrigemPonto.Editado);
                }

                var result = await _poçoWriteOnlyRepository.AtualizarPoço(poço);

                if (result)
                {
                    poço = await _poçoReadOnlyRepository.ObterPoço(idPoço);

                    var perfisAlterados = await _pipeUseCase.Execute(poço, litologia);

                    return EditarLitologiaOutput.LitologiaEditada(ObterLitologiaEmPoço(poço, tipoLitologia), perfisAlterados);
                }

                return EditarLitologiaOutput.LitologiaNãoEditada();
            }
            catch (Exception e)
            {
                return EditarLitologiaOutput.LitologiaNãoEditada(e.Message);
            }
        }
        
        private Litologia ObterLitologiaEmPoço(Poço poço, string tipoLitologia)
        {
            var litologiasComTipoRecebido = poço.Litologias.Where(lito => lito.Classificação.Nome == tipoLitologia).ToList();

            if (litologiasComTipoRecebido.Count() == 0)
            {
                return null;
            }

            return litologiasComTipoRecebido[0];
        }
    }
}
