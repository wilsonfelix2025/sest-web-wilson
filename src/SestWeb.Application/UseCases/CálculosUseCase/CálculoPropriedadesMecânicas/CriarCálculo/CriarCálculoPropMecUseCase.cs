using SestWeb.Application.Repositories;
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

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarCálculo
{
    public class CriarCálculoPropMecUseCase : ICriarCálculoPropMecUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoPropriedadesMecânicasFactory _cálculoFactory;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;

        public CriarCálculoPropMecUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, ICálculoPropriedadesMecânicasFactory cálculoFactory
            , ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoFactory correlaçãoFactory)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _cálculoFactory = cálculoFactory;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoFactory = correlaçãoFactory;
        }

        public async Task<CriarCálculoPropMecOutput> Execute(CriarCalculoPropMecInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return CriarCálculoPropMecOutput.CálculoPropMecNãoCriado("Poço não encontrado");

                //preparação para as entradas para a factory
                var listaPerfisEntrada = await PreencherPerfisEntrada(input.ListaIdPerfisEntrada);
                var listaCorrelações = await PreencherCorrelações(input.Correlações);
                var listaTrechos = await PreencherTrechos(input.Trechos);
                var listaRegiões = await PreencherListaRegiões(input.Regiões);
                var listaPerfisSaída = PreencherListaPerfisSaída(listaCorrelações, poço, input, listaRegiões);
                var listaRegiõesFront = PreencherListaRegiãoFront(input.Regiões);


                //retorna se a entidade calculo criada está valida ou não
                var result = _cálculoFactory.CreateCálculoPropriedadesMecânicas(input.Nome, listaCorrelações.ToList(), "PropriedadesMecânicas", listaPerfisEntrada.ToList(), listaPerfisSaída
                    , poço.Trajetória, poço.ObterLitologiaPadrão(), listaTrechos, poço.DadosGerais.Geometria, poço.DadosGerais, null, string.Empty, listaRegiões, listaRegiõesFront, out var calc);

                //se for valida, executa o calculo
                if (result.IsValid)
                    calc.Execute();
                else
                    return CriarCálculoPropMecOutput.CálculoPropMecNãoCriado(string.Join(";\n", result.Errors));

                //validção dos irmãos
                var validator = new CálculoValidator(_poçoReadOnlyRepository, poço);
                var resultUseCase = validator.Validate((Cálculo)calc);

                if (resultUseCase.IsValid)
                {
                    await _poçoWriteOnlyRepository.CriarCálculo(poço, (Cálculo)calc, "PropriedadesMecânicas");
                    return CriarCálculoPropMecOutput.CálculoPorpMecCriado(calc);
                }

                return CriarCálculoPropMecOutput.CálculoPropMecNãoCriado(string.Join(";\n", result.Errors));
            }
            catch (Exception e)
            {
                return CriarCálculoPropMecOutput.CálculoPropMecNãoCriado(e.Message);
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
                var biot = await _correlaçãoReadOnlyRepository.ObterCorrelaçãoPeloNome(região.BIOTCorrelação);
                var grupoLito = GrupoLitologico.GetFromName(região.GrupoLitológico);

                if (ucs == null)
                    throw new ArgumentNullException(
                        $"correlação de ucs de {região.GrupoLitológico}, deve ser selecionada.");

                if (coesa == null)
                    throw new ArgumentNullException(
                        $"correlação de coesão de {região.GrupoLitológico}, deve ser selecionada.");

                if (angat == null)
                    throw new ArgumentNullException(
                        $"correlação de ângulo de atrito de {região.GrupoLitológico}, deve ser selecionada.");

                if (restr == null)
                    throw new ArgumentNullException(
                        $"correlação de resistência a tração de {região.GrupoLitológico}, deve ser selecionada.");

                var regiãoEntity = new CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas(grupoLito, ucs, coesa, angat, restr, biot);
                
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

        private IList<PerfilBase> PreencherListaPerfisSaída(IReadOnlyCollection<ICorrelação> correlações, Poço poço, CriarCalculoPropMecInput input, List<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> listaRegiões)
        {
            var listaPerfil = new List<PerfilBase>();
            foreach (var corr in correlações)
            {
                foreach (var tipo in corr.PerfisSaída.Tipos)
                {
                    if (!listaPerfil.Select(s => s.Mnemonico).Contains(tipo))
                    {
                        var perfil = PerfisFactory.Create(tipo, tipo + "_" + input.Nome, poço.Trajetória, poço.ObterLitologiaPadrão());
                        
                        listaPerfil.Add(perfil);
                    }
                }
            }

            foreach (var região in listaRegiões)
            {
                foreach (var tipo in região.Angat.PerfisSaída.Tipos)
                {
                    if (!listaPerfil.Select(s => s.Mnemonico).Contains(tipo))
                    {
                        var perfil = PerfisFactory.Create(tipo, tipo + "_" + input.Nome, poço.Trajetória, poço.ObterLitologiaPadrão());

                        listaPerfil.Add(perfil);
                    }
                }
                foreach (var tipo in região.Coesa.PerfisSaída.Tipos)
                {
                    if (!listaPerfil.Select(s => s.Mnemonico).Contains(tipo))
                    {
                        var perfil = PerfisFactory.Create(tipo, tipo + "_" + input.Nome, poço.Trajetória, poço.ObterLitologiaPadrão());

                        listaPerfil.Add(perfil);
                    }
                }
                foreach (var tipo in região.Restr.PerfisSaída.Tipos)
                {
                    if (!listaPerfil.Select(s => s.Mnemonico).Contains(tipo))
                    {
                        var perfil = PerfisFactory.Create(tipo, tipo + "_" + input.Nome, poço.Trajetória, poço.ObterLitologiaPadrão());

                        listaPerfil.Add(perfil);
                    }
                }
                foreach (var tipo in região.Ucs.PerfisSaída.Tipos)
                {
                    if (!listaPerfil.Select(s => s.Mnemonico).Contains(tipo))
                    {
                        var perfil = PerfisFactory.Create(tipo, tipo + "_" + input.Nome, poço.Trajetória, poço.ObterLitologiaPadrão());

                        listaPerfil.Add(perfil);
                    }
                }
            }

            return listaPerfil;
        }

    }
}
