using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Application.Validadores;
using SestWeb.Domain.DTOs.Cálculo;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo;
using SestWeb.Domain.Entities.Cálculos.Perfis.Factory;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Helpers;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.CriarCálculo
{
    internal class CriarCálculoPerfisUseCase : ICriarCálculoPerfisUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoPerfisFactory _cálculoFactory;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;

        public CriarCálculoPerfisUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, ICálculoPerfisFactory cálculoFactory
            , ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoFactory correlaçãoFactory)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _cálculoFactory = cálculoFactory;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoFactory = correlaçãoFactory;
        }

        public async Task<CriarCálculoPerfisOutput> Execute(CriarCalculoPerfisInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return CriarCálculoPerfisOutput.CálculoPerfisNãoCriado("Poço não encontrado");

                //preparação para as entradas para a factory
                var listaPerfisEntrada = await PreencherPerfisEntrada(input.ListaIdPerfisEntrada);
                var listaCorrelações = await PreencherCorrelações(input.Correlações);
                var listaVariáveis = PreencherListaVariáveis(input.Parâmetros);
                var listaTrechos = await PreencherTrechos(input.Trechos);
                var listaPerfisSaída = PreencherListaPerfisSaída(listaCorrelações, poço, input.Nome);
                var listaTrechoFront = PreencherListaTrechoFront(input.Trechos);

                //retorna se a entidade calculo criada está valida ou não
                var result = _cálculoFactory.CreateCálculoPerfis(input.Nome, listaCorrelações.ToList(), "Perfis", listaPerfisEntrada.ToList(), listaPerfisSaída
                    , poço.Trajetória, poço.ObterLitologiaPadrão(), listaVariáveis, listaTrechos, poço.DadosGerais.Geometria, poço.DadosGerais, listaTrechoFront, string.Empty, out var calc);

                //se for valida, executa o calculo
                if (result.IsValid)
                    calc.Execute();
                else
                    return CriarCálculoPerfisOutput.CálculoPerfisNãoCriado(string.Join(";\n", result.Errors));

                //validção dos irmãos
                var validator = new CálculoValidator(_poçoReadOnlyRepository, poço);
                var resultUseCase = validator.Validate((Cálculo)calc);

                if (resultUseCase.IsValid)
                {
                    await _poçoWriteOnlyRepository.CriarCálculo(poço, (Cálculo)calc, "Perfis");
                    return CriarCálculoPerfisOutput.CálculoPerfisCriado(calc);
                }

                return CriarCálculoPerfisOutput.CálculoPerfisNãoCriado(string.Join(";\n", result.Errors));
            }
            catch (Exception e)
            {
                return CriarCálculoPerfisOutput.CálculoPerfisNãoCriado(e.Message);
            }
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
                    Correlação = param.Correlação, Parâmetro = param.Parâmetro, Valor = param.Valor
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
                var correlação = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(trecho.Correlação);
                var expressão = correlação.Expressão.Bruta;

                if (trecho.ListaParametros != null && trecho.ListaParametros.Any())
                {
                    foreach (var varTrecho in trecho.ListaParametros)
                    {
                        paramTrecho.Add("trecho" + varTrecho.Parâmetro, varTrecho.Valor.ToDouble());
                        expressão = expressão.Replace(varTrecho.Parâmetro, "trecho" + varTrecho.Parâmetro);
                        _correlaçãoFactory.CreateCorrelação($"Trecho_Correlação", "Cálculo", "Cálculo", "CálculoPerfis", Origem.CálculoDePerfis.ToString(), expressão, out ICorrelação correlaçãoCriada);
                        correlação = (Correlação) correlaçãoCriada;
                    }
                }
                else
                {
                    paramTrecho = null;
                }

                var trechoDto = new TrechoCálculo(correlação, trecho.PmTopo, trecho.PmBase, paramTrecho);

                lista.Add(trechoDto);
            }

            return lista;
        }

        private IList<PerfilBase> PreencherListaPerfisSaída(IReadOnlyCollection<ICorrelação> correlações, Poço poço, string nomeCálculo)
        {
            var listaPerfil = new List<PerfilBase>();
            foreach (var corr in correlações)
            {
                foreach (var tipo in corr.PerfisSaída.Tipos)
                {
                    if (!listaPerfil.Select(s => s.Mnemonico).Contains(tipo))
                    {
                        var perfil = PerfisFactory.Create(tipo, tipo + "_" + nomeCálculo, poço.Trajetória, poço.ObterLitologiaPadrão());
                        if (tipo == "DTS" || tipo == "RHOB")
                            perfil.EditarPodeSerEntradaCálculoPerfis(false);

                        listaPerfil.Add(perfil);
                    }
                }
            }

            return listaPerfil;
        }
    }
}
