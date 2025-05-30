using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using SestWeb.Application.Validadores;
using SestWeb.Domain.DTOs.Cálculo;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Perfis.Factory;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Helpers;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.EditarCálculo
{
    internal class EditarCálculoPerfisUseCase : IEditarCálculoPerfisUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoPerfisFactory _cálculoFactory;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;
        private readonly IPipelineUseCase _pipeUseCase;        
        private ObjectId IdRHOG;
        private ObjectId IdDTMC;


        public EditarCálculoPerfisUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, ICálculoPerfisFactory cálculoFactory
            , ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoFactory correlaçãoFactory
            , IPipelineUseCase useCase)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _cálculoFactory = cálculoFactory;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoFactory = correlaçãoFactory;
            _pipeUseCase = useCase;
        }

        public async Task<EditarCálculoPerfisOutput> Execute(EditarCalculoPerfisInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return EditarCálculoPerfisOutput.CálculoPerfisNãoEditado("Poço não encontrado");

                var cálculo = (Domain.Entities.Cálculos.Perfis.CálculoPerfis)poço.Cálculos.First(c => c.Id.ToString() == input.IdCálculo);

                var listaPerfisEntrada = input.ListaIdPerfisEntrada == null ? await PreencherPerfisEntrada(cálculo.PerfisEntrada.IdPerfis) : await PreencherPerfisEntrada(input.ListaIdPerfisEntrada);
                var listaCorrelações = input.Correlações == null ? await PreencherCorrelações(cálculo.ListaNomesCorrelação) : await PreencherCorrelações(input.Correlações);
                var listaVariáveis = input.Parâmetros == null ? cálculo.Variáveis : PreencherListaVariáveis(input.Parâmetros);
                var listaTrechos = input.Trechos == null ? null : await PreencherTrechos(input.Trechos);
                var listaPerfisSaída = await PreencherListaPerfisSaída(cálculo.PerfisSaída.IdPerfis);
                var listaTrechoFront = input.Trechos == null ? cálculo.TrechosFront : PreencherListaTrechoFront(input.Trechos);
                var nome = input.Nome ?? cálculo.Nome;
                var correlaçãoDoCálculo = PreencherCorrelaçãoDoCálculo(input, cálculo);

                var result = _cálculoFactory.CreateCálculoPerfis(nome, listaCorrelações.ToList(), "Perfis", listaPerfisEntrada.ToList(), listaPerfisSaída
                    , poço.Trajetória, poço.ObterLitologiaPadrão(), listaVariáveis, listaTrechos, poço.DadosGerais.Geometria, poço.DadosGerais, listaTrechoFront, correlaçãoDoCálculo, out var calc);

                //se for valida, executa o calculo
                if (result.IsValid)
                    calc.Execute();
                else
                    return EditarCálculoPerfisOutput.CálculoPerfisNãoEditado(string.Join(";\n", result.Errors));

                //validção dos irmãos
                var validator = new CálculoValidator(_poçoReadOnlyRepository, poço);
                var resultUseCase = validator.Validate((Cálculo)calc);

                if (resultUseCase.IsValid)
                {
                    var perfilRHOG = calc.PerfisSaída.Perfis.First(p => p.Mnemonico == "RHOG");
                    var perfilDTMC = calc.PerfisSaída.Perfis.First(p => p.Mnemonico == "DTMC");
                    perfilDTMC.EditarId(IdDTMC);
                    perfilRHOG.EditarId(IdRHOG);
                    calc.PerfisSaída.IdPerfis.Add(IdDTMC.ToString());
                    calc.PerfisSaída.IdPerfis.Add(IdRHOG.ToString());
                    await _poçoWriteOnlyRepository.EditarCálculo(poço, (Cálculo)calc, input.IdCálculo, "Perfis");

                    var perfisAlterados = await _pipeUseCase.Execute(poço, calc, input.IdCálculo);                  

                    return EditarCálculoPerfisOutput.CálculoPerfisEditado(calc, perfisAlterados);
                }

                return EditarCálculoPerfisOutput.CálculoPerfisNãoEditado(string.Join(";\n", result.Errors));
            }
            catch (Exception e)
            {
                return EditarCálculoPerfisOutput.CálculoPerfisNãoEditado(e.Message);
            }
        }        

        private string PreencherCorrelaçãoDoCálculo(EditarCalculoPerfisInput input, Domain.Entities.Cálculos.Perfis.CálculoPerfis cálculo)
        {
            var correlação = string.Empty;

            if (input.Correlações == null && input.Parâmetros == null && input.Trechos == null)
                correlação = cálculo.CorrelaçãoDoCálculo;

            return correlação;
        }

        private async Task<IReadOnlyCollection<PerfilBase>> PreencherPerfisEntrada(IEnumerable<string> listaIdPerfis)
        {
            var perfisEntrada = new List<PerfilBase>();
            foreach (var id in listaIdPerfis)
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(id);
                perfisEntrada.Add(perfil);
            }

            //TODO (Vanessa Chalub) Implementar método para buscar os perfis de uma vez no mongo
            //var perfisEntrada = await _perfilReadOnlyRepository.ObterPerfisPorListaDeIds(listaIdPerfis);

            return perfisEntrada;
        }

        private List<TrechoDTO> PreencherListaTrechoFront(List<TrechoInput> trechos)
        {
            if (trechos == null || !trechos.Any())
                return null;

            var lista = new List<TrechoDTO>();


            foreach (var trecho in trechos)
            {
                var trechoFront = new TrechoDTO();
                trechoFront.ListaParametros = new List<VariávelDTO>();

                if (trecho.ListaParametros != null && trecho.ListaParametros.Any())
                {
                    foreach (var varTrecho in trecho.ListaParametros)
                    {
                        var param = new VariávelDTO
                        {
                            Parâmetro = varTrecho.Parâmetro,
                            Correlação = varTrecho.Correlação,
                            Valor = varTrecho.Valor
                        };

                        trechoFront.ListaParametros.Add(param);
                    }
                }
                else
                {
                    trechoFront.ListaParametros = null;
                }

                trechoFront.PmTopo = trecho.PmTopo;
                trechoFront.PmBase = trecho.PmBase;
                trechoFront.Correlação = trecho.Correlação;
                trechoFront.TipoPerfil = trecho.TipoPerfil;

                lista.Add(trechoFront);
            }

            return lista;
        }

        private async Task<IReadOnlyCollection<ICorrelação>> PreencherCorrelações(List<string> listaCorrelações)
        {
            var correlações = await _correlaçãoReadOnlyRepository.ObterCorrelaçõesPorNomes(listaCorrelações);

            return correlações;
        }

        private IList<VariávelDTO> PreencherListaVariáveis(IReadOnlyCollection<ParametroInput> parametros)
        {
            if (parametros == null || !parametros.Any())
                return null;

            var lista = new List<VariávelDTO>();

            foreach (var param in parametros)
            {
                var variável = new VariávelDTO
                {
                    Correlação = param.Correlação,
                    Parâmetro = param.Parâmetro,
                    Valor = param.Valor
                };

                lista.Add(variável);
            }

            return lista;
        }

        private async Task<IList<TrechoCálculo>> PreencherTrechos(IReadOnlyCollection<TrechoInput> trechos)
        {
            if (trechos == null || !trechos.Any())
                return null;

            var lista = new List<TrechoCálculo>();

            foreach (var trecho in trechos)
            {
                var paramTrecho = new Dictionary<string, double>();
                if (trecho.ListaParametros != null && trecho.ListaParametros.Any())
                {
                    foreach (var varTrecho in trecho.ListaParametros)
                    {
                        paramTrecho.Add(varTrecho.Parâmetro, varTrecho.Valor.ToDouble());
                    }
                }
                else
                {
                    paramTrecho = null;
                }

                var correlação = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(trecho.Correlação);
                var trechoDto = new TrechoCálculo(correlação, trecho.PmTopo, trecho.PmBase, paramTrecho);

                lista.Add(trechoDto);
            }

            return lista;
        }

        private async Task<IList<PerfilBase>> PreencherListaPerfisSaída(List<string> perfis)
        {
            var listaPerfil = new List<PerfilBase>();

            foreach (var perfil in perfis)
            {
                var perfilSaida = await _perfilReadOnlyRepository.ObterPerfil(perfil);

                //removo os perfis litológicos
                if (perfilSaida.Mnemonico == "RHOG")
                    IdRHOG = perfilSaida.Id;
                else if (perfilSaida.Mnemonico == "DTMC")
                    IdDTMC = perfilSaida.Id;
                else
                    listaPerfil.Add(perfilSaida);
            }


            return listaPerfil;
        }
    }
}
