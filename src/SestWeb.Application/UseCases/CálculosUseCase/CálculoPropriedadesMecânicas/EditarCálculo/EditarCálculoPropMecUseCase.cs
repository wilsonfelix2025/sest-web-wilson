using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using SestWeb.Application.Validadores;
using SestWeb.Domain.DTOs.Cálculo;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.TrechosDeCálculo;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.Factory;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Poço;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.EditarCálculo
{
    public class EditarCálculoPropMecUseCase : IEditarCálculoPropMecUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoPropriedadesMecânicasFactory _cálculoFactory;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;
        private readonly IPipelineUseCase _pipeUseCase;


        public EditarCálculoPropMecUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, ICálculoPropriedadesMecânicasFactory cálculoFactory
            , ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoFactory correlaçãoFactory
            , IPipelineUseCase pipelineUse)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _cálculoFactory = cálculoFactory;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoFactory = correlaçãoFactory;
            _pipeUseCase = pipelineUse;
        }

        public async Task<EditarCálculoPropMecOutput> Execute(EditarCalculoPropMecInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return EditarCálculoPropMecOutput.CálculoPropMecNãoEditado("Poço não encontrado");

                var cálculo = (Domain.Entities.Cálculos.PropriedadesMecânicas.CálculoPropriedadesMecânicas)poço.Cálculos.First(c => c.Id.ToString() == input.IdCálculo);


                //preparação para as entradas para a factory
                var listaPerfisEntrada = input.ListaIdPerfisEntrada == null ? await PreencherPerfisEntrada(cálculo.PerfisEntrada.IdPerfis) : await PreencherPerfisEntrada(input.ListaIdPerfisEntrada);
                var listaCorrelações = input.Correlações == null ? await PreencherCorrelações(cálculo.ListaNomesCorrelação) : await PreencherCorrelações(input.Correlações);
                var listaRegiões = await PreencherListaRegiões(input.Regiões);
                var listaTrechos = await PreencherTrechos(input.Trechos);
                var listaPerfisSaída = await PreencherListaPerfisSaída(cálculo.PerfisSaída.IdPerfis);
                var nome = input.Nome ?? cálculo.Nome;
                var correlaçãoDoCálculo = PreencherCorrelaçãoDoCálculo(input, cálculo);
                var listaRegiõesFront = input.Regiões == null ? cálculo.RegiõesFront : PreencherListaRegiãoFront(input.Regiões);

                //retorna se a entidade calculo criada está valida ou não
                var result = _cálculoFactory.CreateCálculoPropriedadesMecânicas(input.Nome, listaCorrelações.ToList(), "PropriedadesMecânicas", listaPerfisEntrada.ToList(), listaPerfisSaída
                    , poço.Trajetória, poço.ObterLitologiaPadrão(), listaTrechos, poço.DadosGerais.Geometria, poço.DadosGerais, null, correlaçãoDoCálculo, listaRegiões, listaRegiõesFront, out var calc);

                //se for valida, executa o calculo
                if (result.IsValid)
                    calc.Execute();
                else
                    return EditarCálculoPropMecOutput.CálculoPropMecNãoEditado(string.Join(";\n", result.Errors));

                //validção dos irmãos
                var validator = new CálculoValidator(_poçoReadOnlyRepository, poço);
                var resultUseCase = validator.Validate((Cálculo)calc);

                if (resultUseCase.IsValid)
                {
                    await _poçoWriteOnlyRepository.EditarCálculo(poço, (Cálculo)calc, input.IdCálculo, "PropriedadesMecânicas");

                    var perfisAlterados = await _pipeUseCase.Execute(poço, calc, input.IdCálculo);

                    return EditarCálculoPropMecOutput.CálculoPropMecEditado(calc, perfisAlterados);
                }

                return EditarCálculoPropMecOutput.CálculoPropMecNãoEditado(string.Join(";\n", result.Errors));
            }
            catch (Exception e)
            {
                return EditarCálculoPropMecOutput.CálculoPropMecNãoEditado(e.Message);
            }
        }

        private List<RegiãoDTO> PreencherListaRegiãoFront(List<RegiõesInput> regiões)
        {
            var list = new List<RegiãoDTO>();

            foreach (var item in regiões)
            {
                var dto = new RegiãoDTO
                {
                    ANGATCorrelação = item.ANGATCorrelação,
                    CoesãoCorrelação = item.CoesãoCorrelação,
                    GrupoLitológico = item.GrupoLitológico,
                    RESTRCorrelação = item.RESTRCorrelação,
                    UCSCorrelação = item.UCSCorrelação
                };

                list.Add(dto);
            }

            return list;
        }

        //private List<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico> PreencherListaTrechoFront(List<TrechoPropMecInput> trechos)
        //{
        //    if (trechos == null || !trechos.Any())
        //        return null;

        //    var lista = new List<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico>();


        //    foreach (var trecho in trechos)
        //    {
        //        var trechoFront = new TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico();
        //        trechoFront.ListaParametros = new List<VariávelDTO>();

        //        if (trecho.ListaParametros != null && trecho.ListaParametros.Any())
        //        {
        //            foreach (var varTrecho in trecho.ListaParametros)
        //            {
        //                var param = new VariávelDTO
        //                {
        //                    Parâmetro = varTrecho.Parâmetro,
        //                    Correlação = varTrecho.Correlação,
        //                    Valor = varTrecho.Valor
        //                };

        //                trechoFront.ListaParametros.Add(param);
        //            }
        //        }
        //        else
        //        {
        //            trechoFront.ListaParametros = null;
        //        }

        //        trechoFront.PmTopo = trecho.PmTopo;
        //        trechoFront.PmBase = trecho.PmBase;
        //        trechoFront.Correlação = trecho.Correlação;
        //        trechoFront.TipoPerfil = trecho.TipoPerfil;

        //        lista.Add(trechoFront);
        //    }

        //    return lista;
        //}

        private string PreencherCorrelaçãoDoCálculo(EditarCalculoPropMecInput input, Domain.Entities.Cálculos.PropriedadesMecânicas.CálculoPropriedadesMecânicas cálculo)
        {
            var correlação = string.Empty;

            if (input.Correlações == null && input.Regiões == null && input.Trechos == null)
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

            return perfisEntrada;
        }

        private async Task<IReadOnlyCollection<ICorrelação>> PreencherCorrelações(List<string> listaCorrelações)
        {
            var correlações = await _correlaçãoReadOnlyRepository.ObterCorrelaçõesPorNomes(listaCorrelações);

            return correlações;
        }

        private async Task <List<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas>> PreencherListaRegiões(IReadOnlyCollection<RegiõesInput> regiões)
        {
            if (regiões == null || !regiões.Any())
                return null;

            var lista = new List<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas>();

            foreach (var região in regiões)
            {
                var ucs = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(região.UCSCorrelação);
                var coesa = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(região.CoesãoCorrelação);
                var angat = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(região.ANGATCorrelação);
                var restr = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(região.RESTRCorrelação);
                var grupoLito = GrupoLitologico.GetFromName(região.GrupoLitológico);
                var regiãoEntity = new CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas(grupoLito, ucs, coesa, angat, restr);
                
                lista.Add(regiãoEntity);
            }

            return lista;
        }

        private async Task<IList<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico>> PreencherTrechos(IReadOnlyCollection<TrechoPropMecInput> trechos)
        {
            if (trechos == null || !trechos.Any())
                return null;

            var lista = new List<TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico>();

            foreach (var trecho in trechos)
            {
                var ucs = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(trecho.UCSCorrelação);
                var coesa = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(trecho.CoesãoCorrelação);
                var angat = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(trecho.ANGATCorrelação);
                var restr = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(trecho.RESTRCorrelação);
                var biot = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(trecho.BIOTCorrelação);
                var grupoLito = GrupoLitologico.GetFromName(trecho.GrupoLitológico);
                var trechoEntity = new TrechoDeCáculoPropriedadesMecânicasPorGrupoLitológico(grupoLito, trecho.PmTopo, trecho.PmBase, ucs, coesa, angat, restr, biot);

                lista.Add(trechoEntity);
            }

            return lista;
        }

        private async Task<IList<PerfilBase>> PreencherListaPerfisSaída(List<string> perfis)
        {
            var listaPerfil = new List<PerfilBase>();

            foreach (var perfil in perfis)
            {
                var perfilSaida = await _perfilReadOnlyRepository.ObterPerfil(perfil);
                listaPerfil.Add(perfilSaida);
            }

            return listaPerfil;
        }

    }
}
