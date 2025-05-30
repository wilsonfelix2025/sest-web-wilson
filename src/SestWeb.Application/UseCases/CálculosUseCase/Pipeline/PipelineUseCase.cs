using MongoDB.Bson;
using SestWeb.Application.Repositories;
using SestWeb.Domain.DTOs.Cálculo;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD;
using SestWeb.Domain.Entities.Cálculos.ExpoenteD.Factory;
using SestWeb.Domain.Entities.Cálculos.Filtros;
using SestWeb.Domain.Entities.Cálculos.Filtros.Corte;
using SestWeb.Domain.Entities.Cálculos.Filtros.Corte.Factory;
using SestWeb.Domain.Entities.Cálculos.Filtros.LinhaBaseFolhelho;
using SestWeb.Domain.Entities.Cálculos.Filtros.LinhaBaseFolhelho.Factory;
using SestWeb.Domain.Entities.Cálculos.Filtros.Litologia;
using SestWeb.Domain.Entities.Cálculos.Filtros.Litologia.Factory;
using SestWeb.Domain.Entities.Cálculos.Filtros.MediaMovel;
using SestWeb.Domain.Entities.Cálculos.Filtros.MediaMovel.Factory;
using SestWeb.Domain.Entities.Cálculos.Filtros.Simples;
using SestWeb.Domain.Entities.Cálculos.Filtros.Simples.Factory;
using SestWeb.Domain.Entities.Cálculos.Gradientes;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Factory;
using SestWeb.Domain.Entities.Cálculos.Perfis;
using SestWeb.Domain.Entities.Cálculos.Perfis.Factory;
using SestWeb.Domain.Entities.Cálculos.Pipelines;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Factory;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Métodos;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Reservatório;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.Factory;
using SestWeb.Domain.Entities.Cálculos.Sobrecarga;
using SestWeb.Domain.Entities.Cálculos.Sobrecarga.Factory;
using SestWeb.Domain.Entities.Cálculos.Tensões;
using SestWeb.Domain.Entities.Cálculos.Tensões.Factory;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.Correlações.ParâmetroCorrelação;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.Trend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.Pipeline
{
    public class PipelineUseCase : IPipelineUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICálculoPropriedadesMecânicasFactory _cálculoPropMecFactory;
        private readonly ICálculoPerfisFactory _cálculoPerfisFactory;
        private readonly ICálculoGradientesFactory _cálculoGradientesFactory;
        private readonly ICálculoTensõesFactory _cálculoTensõesFactory;
        private readonly ICálculoSobrecargaFactory _cálculoSobrecargaFactory;
        private readonly ICálculoPressãoPorosFactory _cálculoPressãoPorosFactory;
        private readonly ICálculoExpoenteDFactory _cálculoExpoenteDFactory;
        private readonly IFiltroSimplesFactory _filtroSimplesFactory;
        private readonly IFiltroMédiaMóvelFactory _filtroMédiaMóvelFactory;
        private readonly IFiltroLinhaBaseFolhelhoFactory _filtroLinhaBaseFolhelhoFactory;
        private readonly IFiltroLitologiaFactory _filtroLitologiaFactory;
        private readonly IFiltroCorteFactory _filtroCorteFactory;
        private ObjectId IdRHOG;
        private ObjectId IdDTMC;

        public PipelineUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, ICorrelaçãoFactory correlaçãoFactory
            , ICálculoPropriedadesMecânicasFactory cálculoPropMecFactory, IPerfilWriteOnlyRepository perfilWriteOnlyRepository
            , ICálculoPerfisFactory cálculoPerfisFactory, ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository
            , ICálculoGradientesFactory cálculoGradientesFactory, ICálculoTensõesFactory cálculoTensõesFactory
            , ICálculoSobrecargaFactory cálculoSobrecargaFactory, ICálculoPressãoPorosFactory cálculoPressãoPorosFactory
            , ICálculoExpoenteDFactory cálculoExpoenteDFactory, IFiltroSimplesFactory filtroSimplesFactory
            , IFiltroMédiaMóvelFactory filtroMédiaMóvelFactory, IFiltroLinhaBaseFolhelhoFactory filtroLinhaBaseFolhelhoFactory
            , IFiltroLitologiaFactory filtroLitologiaFactory, IFiltroCorteFactory filtroCorteFactory)
        {
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _correlaçãoFactory = correlaçãoFactory;
            _cálculoPropMecFactory = cálculoPropMecFactory;
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
            _cálculoPerfisFactory = cálculoPerfisFactory;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _cálculoGradientesFactory = cálculoGradientesFactory;
            _cálculoTensõesFactory = cálculoTensõesFactory;
            _cálculoSobrecargaFactory = cálculoSobrecargaFactory;
            _cálculoPressãoPorosFactory = cálculoPressãoPorosFactory;
            _cálculoExpoenteDFactory = cálculoExpoenteDFactory;
            _filtroSimplesFactory = filtroSimplesFactory;
            _filtroMédiaMóvelFactory = filtroMédiaMóvelFactory;
            _filtroLinhaBaseFolhelhoFactory = filtroLinhaBaseFolhelhoFactory;
            _filtroLitologiaFactory = filtroLitologiaFactory;
            _filtroCorteFactory = filtroCorteFactory;
        }

        public async Task<List<PerfilBase>> Execute(Poço poço, ICálculo cálculo, string idCálculo)
        {
            try
            {
                var aux = poço.Cálculos.Where(c => c.GrupoCálculo == GrupoCálculo.FiltroCorte
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroLinhaBaseFolhelho
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroLitologia
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroMédiaMóvel
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroSimples).ToList();
                List<IFiltro> filtros = new List<IFiltro>();
                foreach (var item in aux)
                {
                    filtros.Add(item as Filtro);
                }

                List<ICálculo> cálculosDoPoço = new List<ICálculo>();
                foreach (var item in poço.Cálculos)
                {
                    if (!string.IsNullOrEmpty(idCálculo) && item.Id.ToString() == idCálculo)
                        continue;

                    cálculosDoPoço.Add(item);
                }
                cálculosDoPoço.Add(cálculo);


                var gerenciadorPipe = new PipelinesExecutor(cálculosDoPoço, filtros);
                var cálculosDependentes = gerenciadorPipe.GetDependentCalculus(cálculo);
                var cálculosPreenchidos = await GetCalculos(cálculosDependentes, poço);
                var perfisAlterados = gerenciadorPipe.Recalculate(cálculosPreenchidos);

                foreach (var perfil in perfisAlterados)
                {
                    if (perfil.Mnemonico == "RHOG")
                        perfil.EditarId(IdRHOG);
                    else if (perfil.Mnemonico == "DTMC")
                        perfil.EditarId(IdDTMC);

                    await _perfilWriteOnlyRepository.AtualizarPerfil(perfil);
                }

                return perfisAlterados.ToList();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<PerfilBase>> Execute(List<Cálculo> cálculos, Poço poço)
        {
            try
            {
                var aux = cálculos.Where(c => c.GrupoCálculo == GrupoCálculo.FiltroCorte
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroLinhaBaseFolhelho
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroLitologia
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroMédiaMóvel
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroSimples).ToList();
                List<IFiltro> filtros = new List<IFiltro>();
                foreach (var item in aux)
                {
                    filtros.Add(item as Filtro);
                }

                List<ICálculo> cálculosDoPoço = new List<ICálculo>();
                foreach (var item in cálculos)
                {       
                    cálculosDoPoço.Add(item);
                }
                
                var gerenciadorPipe = new PipelinesExecutor(cálculosDoPoço, filtros);
                var cálculosPreenchidos = await GetCalculos(cálculosDoPoço, poço);
                var perfisAlterados = gerenciadorPipe.Recalculate(cálculosPreenchidos);

                foreach (var perfil in perfisAlterados)
                {
                    if (perfil.Mnemonico == "RHOG")
                        perfil.EditarId(IdRHOG);
                    else if (perfil.Mnemonico == "DTMC")
                        perfil.EditarId(IdDTMC);
                }

                return perfisAlterados.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<PerfilBase>> Execute(Poço poço, PerfilBase perfil)
        {
            try
            {
                var aux = poço.Cálculos.Where(c => c.GrupoCálculo == GrupoCálculo.FiltroCorte
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroLinhaBaseFolhelho
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroLitologia
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroMédiaMóvel
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroSimples).ToList();
                List<IFiltro> filtros = new List<IFiltro>();
                foreach (var item in aux)
                {
                    filtros.Add(item as Filtro);
                }

                List<ICálculo> cálculosDoPoço = new List<ICálculo>();
                foreach (var item in poço.Cálculos)
                {         
                    cálculosDoPoço.Add(item);
                }


                var gerenciadorPipe = new PipelinesExecutor(cálculosDoPoço, filtros);
                var cálculosDependentes = gerenciadorPipe.GetDependentCalculus(perfil);
                var cálculosPreenchidos = await GetCalculos(cálculosDependentes, poço);
                var perfisAlterados = gerenciadorPipe.Recalculate(cálculosPreenchidos);

                foreach (var perfilAlt in perfisAlterados)
                {
                    await _perfilWriteOnlyRepository.AtualizarPerfil(perfilAlt);
                }

                return perfisAlterados.ToList();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<PerfilBase>> Execute(Poço poço, ILitologia litologia)
        {
            try
            {
                var aux = poço.Cálculos.Where(c => c.GrupoCálculo == GrupoCálculo.FiltroCorte
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroLinhaBaseFolhelho
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroLitologia
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroMédiaMóvel
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroSimples).ToList();
                List<IFiltro> filtros = new List<IFiltro>();
                foreach (var item in aux)
                {
                    filtros.Add(item as Filtro);
                }

                List<ICálculo> cálculosDoPoço = new List<ICálculo>();
                foreach (var item in poço.Cálculos)
                {
                    cálculosDoPoço.Add(item);
                }


                var gerenciadorPipe = new PipelinesExecutor(cálculosDoPoço, filtros);
                var cálculosDependentes = gerenciadorPipe.GetDependentCalculus(litologia);
                var cálculosPreenchidos = await GetCalculos(cálculosDependentes, poço);

                foreach (var calc in cálculosPreenchidos)
                {
                    if (calc.Litologia.Id != litologia.Id)
                        cálculosPreenchidos.Remove(calc);
                }

                var perfisAlterados = gerenciadorPipe.Recalculate(cálculosPreenchidos);

                foreach (var perfil in perfisAlterados)
                {
                    if (perfil.Mnemonico == "RHOG")
                        perfil.EditarId(IdRHOG);
                    else if (perfil.Mnemonico == "DTMC")
                        perfil.EditarId(IdDTMC);

                    await _perfilWriteOnlyRepository.AtualizarPerfil(perfil);
                }

                return perfisAlterados.ToList();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<PerfilBase>> Execute(Poço poço, Trend trend)
        {
            try
            {
                var aux = poço.Cálculos.Where(c => c.GrupoCálculo == GrupoCálculo.FiltroCorte
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroLinhaBaseFolhelho
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroLitologia
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroMédiaMóvel
                                                        || c.GrupoCálculo == GrupoCálculo.FiltroSimples).ToList();
                List<IFiltro> filtros = new List<IFiltro>();
                foreach (var item in aux)
                {
                    filtros.Add(item as Filtro);
                }

                List<ICálculo> cálculosDoPoço = new List<ICálculo>();
                foreach (var item in poço.Cálculos)
                {
                    cálculosDoPoço.Add(item);
                }

                var gerenciadorPipe = new PipelinesExecutor(cálculosDoPoço, filtros);
                var cálculosDependentes = gerenciadorPipe.GetDependentCalculus(trend, poço.Perfis);
                var cálculosPreenchidos = await GetCalculos(cálculosDependentes, poço);
                var perfisAlterados = gerenciadorPipe.Recalculate(cálculosPreenchidos);

                foreach (var perfil in perfisAlterados)
                {
                    if (perfil.Mnemonico == "RHOG")
                        perfil.EditarId(IdRHOG);
                    else if (perfil.Mnemonico == "DTMC")
                        perfil.EditarId(IdDTMC);

                    await _perfilWriteOnlyRepository.AtualizarPerfil(perfil);
                }

                return perfisAlterados.ToList();

            }
            catch (Exception)
            {
                throw;
            }
        }


        private async Task<List<ICálculo>> GetCalculos(List<ICálculo> cálculos, Poço poço)
        {
            var cálculosPreenchidos = new List<ICálculo>();

            foreach (var calc in cálculos)
            {
                switch (calc.GrupoCálculo)
                {
                    case GrupoCálculo.Perfis:
                        var cálculoPreenchidoPerfil = await PreencherPerfis(calc, poço);
                        cálculosPreenchidos.Add(cálculoPreenchidoPerfil);
                        break;
                    case GrupoCálculo.Sobrecarga:
                        var cálculoPreenchidoSobrecarga = await PreencherSobrecarga(calc, poço);
                        cálculosPreenchidos.Add(cálculoPreenchidoSobrecarga);
                        break;
                    case GrupoCálculo.PressãoPoros:
                        var cálculoPreenchidoPP = await PreencherPP(calc, poço);
                        cálculosPreenchidos.Add(cálculoPreenchidoPP);
                        break;
                    case GrupoCálculo.PropriedadesMecânicas:
                        var cálculoPreenchidoPropMec = await PreencherPropMec(calc, poço);
                        cálculosPreenchidos.Add(cálculoPreenchidoPropMec);
                        break;
                    case GrupoCálculo.Tensões:
                        var cálculoPreenchidoTensões = await PreencherTensões(calc, poço);
                        cálculosPreenchidos.Add(cálculoPreenchidoTensões);
                        break;
                    case GrupoCálculo.Gradientes:
                        var cálculoPreenchidoGradientes = await PreencherGradientes(calc, poço);
                        cálculosPreenchidos.Add(cálculoPreenchidoGradientes);
                        break;
                    case GrupoCálculo.ExpoenteD:
                        var cálculoPreenchidoExpoenteD = await PreencherExpoenteD(calc, poço);
                        cálculosPreenchidos.Add(cálculoPreenchidoExpoenteD);
                        break;
                    case GrupoCálculo.FiltroSimples:
                        var filtroSimplesPreenchido = await PreencherFiltroSimples(calc, poço);
                        cálculosPreenchidos.Add(filtroSimplesPreenchido);
                        break;
                    case GrupoCálculo.FiltroMédiaMóvel:
                        var filtroMédiaMóvelPreenchido = await PreencherFiltroMédiaMóvel(calc, poço);
                        cálculosPreenchidos.Add(filtroMédiaMóvelPreenchido);
                        break;
                    case GrupoCálculo.FiltroLitologia:
                        var filtroLitoPreenchido = await PreencherFiltroLitologia(calc, poço);
                        cálculosPreenchidos.Add(filtroLitoPreenchido);                        
                        break;
                    case GrupoCálculo.FiltroLinhaBaseFolhelho:
                        var filtroLBFPreenchido = await PreencherFiltroLBF(calc, poço);
                        cálculosPreenchidos.Add(filtroLBFPreenchido);
                        break;
                    case GrupoCálculo.FiltroCorte:
                        var filtroCortePreenchido = await PreencherFiltroCorte(calc, poço);
                        cálculosPreenchidos.Add(filtroCortePreenchido);
                        break;                        
                }
            }

            return cálculosPreenchidos;
        }


        private async Task<ICálculo> PreencherFiltroCorte(ICálculo calc, Poço poço)
        {
            var filtro = (FiltroCorte)calc;
            var listaPerfisEntrada = await PreencherPerfisEntrada(calc.PerfisEntrada.IdPerfis);
            var listaPerfisSaída = await PreencherListaPerfisSaída(calc.PerfisSaída.IdPerfis);

            var preencherCálculo = _filtroCorteFactory.CreateFiltroCorte(calc.Nome, "FiltroCorte", listaPerfisEntrada.First(), listaPerfisSaída.First()
                , poço.Trajetória, poço.ObterLitologiaPadrão(), filtro.LimiteInferior, filtro.LimiteSuperior, filtro.TipoCorte
                , out IFiltro cálculoPreenchido);

            return cálculoPreenchido;
        }

        private async Task<ICálculo> PreencherFiltroLitologia(ICálculo calc, Poço poço)
        {
            var filtro = (FiltroLitologia)calc;
            var listaPerfisEntrada = await PreencherPerfisEntrada(calc.PerfisEntrada.IdPerfis);
            var listaPerfisSaída = await PreencherListaPerfisSaída(calc.PerfisSaída.IdPerfis);

            var preencherCálculo = _filtroLitologiaFactory.CreateFiltroLitologia(calc.Nome, "FiltroLitologia", listaPerfisEntrada.First(), listaPerfisSaída.First()
                , poço.Trajetória, poço.ObterLitologiaPadrão(), filtro.LimiteInferior, filtro.LimiteSuperior, filtro.TipoCorte
                , filtro.Litologias, out IFiltro cálculoPreenchido);

            return cálculoPreenchido;
        }

        private async Task<ICálculo> PreencherFiltroLBF(ICálculo calc, Poço poço)
        {
            var filtro = (FiltroLinhaBaseFolhelho)calc;
            var listaPerfisEntrada = await PreencherPerfisEntrada(calc.PerfisEntrada.IdPerfis);
            var listaPerfisSaída = await PreencherListaPerfisSaída(calc.PerfisSaída.IdPerfis);
            var perfilLBF = await _perfilReadOnlyRepository.ObterPerfil(filtro.IdPerfilLBF);

            var preencherCálculo = _filtroLinhaBaseFolhelhoFactory.CreateFiltroLinhaBaseFolhelho(calc.Nome, "FiltroLinhaBaseFolhelho", listaPerfisEntrada.First(), listaPerfisSaída.First()
                , poço.Trajetória, poço.ObterLitologiaPadrão(), filtro.LimiteInferior, filtro.LimiteSuperior, filtro.TipoCorte
                , perfilLBF, out IFiltro cálculoPreenchido);

            return cálculoPreenchido;
        }

        private async Task<ICálculo> PreencherFiltroMédiaMóvel(ICálculo calc, Poço poço)
        {
            var filtro = (FiltroMédiaMóvel)calc;
            var listaPerfisEntrada = await PreencherPerfisEntrada(calc.PerfisEntrada.IdPerfis);
            var listaPerfisSaída = await PreencherListaPerfisSaída(calc.PerfisSaída.IdPerfis);

            var preencherCálculo = _filtroMédiaMóvelFactory.CreateFiltroMédiaMóvel(calc.Nome, "FiltroMédiaMóvel", listaPerfisEntrada.First(), listaPerfisSaída.First()
                , poço.Trajetória, poço.ObterLitologiaPadrão(), filtro.LimiteInferior, filtro.LimiteSuperior, filtro.TipoCorte
                , filtro.NúmeroPontos, out IFiltro cálculoPreenchido);

            return cálculoPreenchido;
        }

        private async Task<ICálculo> PreencherFiltroSimples(ICálculo calc, Poço poço)
        {
            var filtroSimples = (FiltroSimples)calc;
            var listaPerfisEntrada = await PreencherPerfisEntrada(calc.PerfisEntrada.IdPerfis);
            var listaPerfisSaída = await PreencherListaPerfisSaída(calc.PerfisSaída.IdPerfis);

            var preencherCálculo = _filtroSimplesFactory.CreateFiltroSimples(calc.Nome, "FiltroSimples", listaPerfisEntrada.First(), listaPerfisSaída.First()
                , poço.Trajetória, poço.ObterLitologiaPadrão(), filtroSimples.LimiteInferior, filtroSimples.LimiteSuperior, filtroSimples.TipoCorte
                , filtroSimples.DesvioMáximo, out IFiltro cálculoPreenchido);

            return cálculoPreenchido;
        }

        private async Task<ICálculo> PreencherExpoenteD(ICálculo calc, Poço poço)
        {
            var calcExpoenteD = (Domain.Entities.Cálculos.ExpoenteD.CálculoExpoenteD)calc;
            var listaPerfisEntrada = await PreencherPerfisEntrada(calc.PerfisEntrada.IdPerfis);
            var listaPerfisSaída = await PreencherListaPerfisSaída(calc.PerfisSaída.IdPerfis);

            var preencherCálculo = _cálculoExpoenteDFactory.CreateCálculoExpoenteD(calc.Nome, "ExpoenteD"
               , listaPerfisEntrada.ToList(), listaPerfisSaída, poço.Trajetória
               , poço.ObterLitologiaPadrão(), string.Empty, out ICálculoExpoenteD cálculoPreenchido);

            return cálculoPreenchido;
        }

        private async Task<ICálculo> PreencherPP(ICálculo calc, Poço poço)
        {
            var calcPP = (Domain.Entities.Cálculos.PressãoPoros.CálculoPressãoPoros)calc;
            var listaPerfisEntrada = await PreencherPerfisEntrada(calc.PerfisEntrada.IdPerfis);
            var listaPerfisSaída = await PreencherListaPerfisSaída(calc.PerfisSaída.IdPerfis);
            var parametrosCorrelação = PreencherParametrosCorrelação(calcPP);
            DadosReservatório reservatório = null;

            switch (calcPP.MétodoCálculo)
            {
                case Domain.Entities.Cálculos.PressãoPoros.Base.CorrelaçãoPressãoPoros.EatonDTC:
                    var auxCalc1 = (CálculoPressãoPorosEatonDTCFiltrado) calc;
                    reservatório = auxCalc1.DadosReservatório;
                    break;
                case Domain.Entities.Cálculos.PressãoPoros.Base.CorrelaçãoPressãoPoros.EatonExpoenteD:
                    var auxCalc2 = (CálculoPressãoPorosEatonExpoenteDFiltrado)calc;
                    reservatório = auxCalc2.DadosReservatório;
                    break;
                case Domain.Entities.Cálculos.PressãoPoros.Base.CorrelaçãoPressãoPoros.EatonResistividade:
                    var auxCalc3 = (CálculoPressãoPorosEatonResistividadeFiltrado)calc;
                    reservatório = auxCalc3.DadosReservatório;
                    break;
                case Domain.Entities.Cálculos.PressãoPoros.Base.CorrelaçãoPressãoPoros.Hidrostática:
                case Domain.Entities.Cálculos.PressãoPoros.Base.CorrelaçãoPressãoPoros.Gradiente:
                    reservatório = null;
                    break;
                default:
                    break;
            }

            var preencherCálculo = _cálculoPressãoPorosFactory.CreateCálculoPressãoPoros(calc.Nome, "PressãoPoros"
               , listaPerfisEntrada.ToList(), listaPerfisSaída, calcPP.MétodoCálculo, parametrosCorrelação, poço.Trajetória
               , poço.ObterLitologiaPadrão(), poço.DadosGerais, reservatório, out ICálculoPressãoPoros cálculoPreenchido);

            return cálculoPreenchido;
        }

        public List<ParâmetroCorrelação> PreencherParametrosCorrelação(Domain.Entities.Cálculos.PressãoPoros.CálculoPressãoPoros calc)
        {
            var parâmetrosCorrelação = new List<ParâmetroCorrelação>();           
                parâmetrosCorrelação.Add(new ParâmetroCorrelação("Gn", calc.Gn));
                parâmetrosCorrelação.Add(new ParâmetroCorrelação("Exp", calc.Exp));
                parâmetrosCorrelação.Add(new ParâmetroCorrelação("Gpph", calc.Gpph));           

            return parâmetrosCorrelação;
        }

        private async Task<ICálculo> PreencherSobrecarga(ICálculo calc, Poço poço)
        {
            var calcSobrecarga = (Domain.Entities.Cálculos.Sobrecarga.CálculoSobrecarga)calc;
            var listaPerfisEntrada = await PreencherPerfisEntrada(calc.PerfisEntrada.IdPerfis);
            var listaPerfisSaída = await PreencherListaPerfisSaída(calc.PerfisSaída.IdPerfis);

            var preencherCálculo = _cálculoSobrecargaFactory.CreateCálculoSobrecarga(calc.Nome, "Sobrecarga"
               , listaPerfisEntrada.ToList(), listaPerfisSaída, poço.Trajetória, poço.ObterLitologiaPadrão()
               , poço.DadosGerais, out ICálculoSobrecarga cálculoPreenchido);

            return cálculoPreenchido;
        }

        private async Task<ICálculo> PreencherTensões(ICálculo calc, Poço poço)
        {
            var calcTensões = (CálculoTensões)calc;
            var listaPerfisEntrada = await PreencherPerfisEntrada(calc.PerfisEntrada.IdPerfis);
            var listaPerfisSaída = await PreencherListaPerfisSaída(calc.PerfisSaída.IdPerfis);

            var preencherCálculo = _cálculoTensõesFactory.CreateCálculoTensões(calc.Nome, calcTensões.MetodologiaTHORmin, "Tensões"
               , listaPerfisEntrada.ToList(), listaPerfisSaída, poço.Trajetória, poço.ObterLitologiaPadrão(), calcTensões.ParâmetrosLotDTO
               , calcTensões.Depleção, calcTensões.Coeficiente, calcTensões.Breakout, calcTensões.RelaçãoTensão, calcTensões.FraturasTrechosVerticais, poço.DadosGerais.Geometria
               , poço.DadosGerais, calcTensões.MetodologiaTHORmax, out ICálculoTensões cálculoPreenchido);

            return cálculoPreenchido;
        }

        private async Task<ICálculo> PreencherGradientes(ICálculo calc, Poço poço)
        {
            var calcGradiente = (Domain.Entities.Cálculos.Gradientes.CálculoGradientes)calc;
            var listaPerfisEntrada = await PreencherPerfisEntrada(calc.PerfisEntrada.IdPerfis);
            var listaPerfisSaída = await PreencherListaPerfisSaída(calc.PerfisSaída.IdPerfis);

            var preencherCálculo = _cálculoGradientesFactory.CreateCálculoGradientes(calc.Nome, "Gradientes"
               , listaPerfisEntrada.ToList(), listaPerfisSaída, poço.Trajetória, poço.ObterLitologiaPadrão(), calcGradiente.Dadosmalha
               , calcGradiente.EntradaColapsos, out ICálculoGradientes cálculoPreenchido);

            return cálculoPreenchido;
        }
      
        private async Task<ICálculo> PreencherPerfis(ICálculo calc, Poço poço)
        {
            var calcPerfil = (Domain.Entities.Cálculos.Perfis.CálculoPerfis)calc;
            var listaPerfisEntrada = await PreencherPerfisEntrada(calc.PerfisEntrada.IdPerfis);
            var listaPerfisSaída = await PreencherListaPerfisSaída(calc.PerfisSaída.IdPerfis, calc.GrupoCálculo);
            var listaCorrelações = await PreencherCorrelações(calcPerfil.ListaNomesCorrelação);


            var preencherCálculo = _cálculoPerfisFactory.CreateCálculoPerfis(calcPerfil.Nome, listaCorrelações.ToList(), "Perfis"
               , listaPerfisEntrada.ToList(), listaPerfisSaída, poço.Trajetória, poço.ObterLitologiaPadrão(), calcPerfil.Variáveis, null 
               , poço.DadosGerais.Geometria, poço.DadosGerais, calcPerfil.TrechosFront, calcPerfil.CorrelaçãoDoCálculo, out ICálculoPerfis cálculoPreenchido);

            return cálculoPreenchido;
        }

        private async Task<IReadOnlyCollection<ICorrelação>> PreencherCorrelações(List<string> listaCorrelações)
        {
            var correlações = await _correlaçãoReadOnlyRepository.ObterCorrelaçõesPorNomes(listaCorrelações);

            return correlações;
        }

        private async Task<ICálculo> PreencherPropMec(ICálculo calc, Poço poço)
        {
            var calcPropMec = (Domain.Entities.Cálculos.PropriedadesMecânicas.CálculoPropriedadesMecânicas)calc;
            var listaPerfisEntrada = await PreencherPerfisEntrada(calc.PerfisEntrada.IdPerfis);
            var listaPerfisSaída = await PreencherListaPerfisSaída(calc.PerfisSaída.IdPerfis);

            var preencherCálculo = _cálculoPropMecFactory.CreateCálculoPropriedadesMecânicas(calcPropMec.Nome, null, "PropriedadesMecânicas"
                , listaPerfisEntrada.ToList(), listaPerfisSaída, poço.Trajetória, poço.ObterLitologiaPadrão(), calcPropMec.Trechos
                , poço.DadosGerais.Geometria, poço.DadosGerais, calcPropMec.TrechosFront, calcPropMec.CorrelaçãoDoCálculo
                , calcPropMec.Regiões, calcPropMec.RegiõesFront, out ICálculoPropriedadesMecânicas cálculoPreenchido);

            return cálculoPreenchido;
        }

        private object PreencherRegiões(List<RegiãoDTO> regiõesFront)
        {
            throw new NotImplementedException();
        }

        private async Task<IList<PerfilBase>> PreencherListaPerfisSaída(List<string> perfis, GrupoCálculo grupoCálculo = GrupoCálculo.Indefinido)
        {
            var listaPerfil = new List<PerfilBase>();

            foreach (var perfil in perfis)
            {            
                var perfilSaida = await _perfilReadOnlyRepository.ObterPerfil(perfil);

                //removo os perfis litológicos
                if (grupoCálculo == GrupoCálculo.Perfis && perfilSaida.Mnemonico == "RHOG")
                {
                    IdRHOG = perfilSaida.Id;
                    continue;
                } else if (grupoCálculo == GrupoCálculo.Perfis && perfilSaida.Mnemonico == "DTMC")
                {
                    IdDTMC = perfilSaida.Id;
                    continue;
                }


                listaPerfil.Add(perfilSaida);
            }

            return listaPerfil;
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


    }
}
