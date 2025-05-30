using SestWeb.Application.Repositories;
using SestWeb.Application.Validadores;
using SestWeb.Domain.DTOs.Cálculo.Gradientes;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Gradientes;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Colapsos;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Factory;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;
using SestWeb.Domain.Entities.Correlações.Base.Factory;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.Recalcular
{
    public class RecalcularCálculoGradientesUseCase : IRecalcularCálculoGradientesUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoGradientesFactory _cálculoFactory;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;

        public RecalcularCálculoGradientesUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, ICálculoGradientesFactory cálculoFactory
            , ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoFactory correlaçãoFactory)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _cálculoFactory = cálculoFactory;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoFactory = correlaçãoFactory;
        }

        public async Task<RecalcularCálculoGradientesOutput> Execute(RecalcularCálculoGradientesInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return RecalcularCálculoGradientesOutput.CálculoNãoRecalculado("Poço não encontrado");

                var calc = poço.Cálculos.Single(c => c.Id.ToString() == input.IdCálculo) as ICálculoGradientes;

                ////preparação para as entradas para a factory
                var listaPerfisEntrada = await PreencherPerfisEntrada(calc.PerfisEntrada.IdPerfis);
                var listaPerfisSaída = await PreencherListaPerfisSaída(calc.PerfisSaída.IdPerfis, calc.Nome);
                var dadosMalha = PreencherDadosMalha(calc);
                var entradasColapsos = PreencherEntradasColapsos(calc);

                ////retorna se a entidade calculo criada está valida ou não
                var result = _cálculoFactory.CreateCálculoGradientes(calc.Nome, "Gradientes"
                    , listaPerfisEntrada.ToList(), listaPerfisSaída, poço.Trajetória, poço.ObterLitologiaPadrão()
                    , dadosMalha, entradasColapsos, out var calculo);

                ////se for valida, executa o calculo
                if (result.IsValid)
                    calculo.Execute();
                else
                    return RecalcularCálculoGradientesOutput.CálculoNãoRecalculado(string.Join(";\n", result.Errors));

                //validção dos irmãos
                var validator = new CálculoValidator(_poçoReadOnlyRepository, poço);
                var resultUseCase = validator.Validate((Cálculo)calculo);

                if (resultUseCase.IsValid)
                {
                    await _poçoWriteOnlyRepository.EditarCálculo(poço, (Cálculo)calculo, input.IdCálculo, "Gradientes");
                    return RecalcularCálculoGradientesOutput.CálculoRecalculado(calculo);
                }

                return RecalcularCálculoGradientesOutput.CálculoNãoRecalculado(string.Join(";\n", result.Errors));
            }
            catch (Exception e)
            {
                return RecalcularCálculoGradientesOutput.CálculoNãoRecalculado(e.Message);
            }
        }

        private EntradasColapsosDTO PreencherEntradasColapsos(ICálculoGradientes cálculo)
        {
            var input = cálculo.EntradaColapsos;

            var entradas = new EntradasColapsosDTO
            {
                AreaPlastificada = input.AreaPlastificada,
                CalcularFraturasColapsosSeparadamente = input.CalcularFraturasColapsosSeparadamente,
                FluidoPenetrante = input.FluidoPenetrante,
                HabilitarFiltroAutomatico = input.HabilitarFiltroAutomatico,
                IncluirEfeitosFísicosQuímicos = input.IncluirEfeitosFísicosQuímicos,
                IncluirEfeitosTérmicos = input.IncluirEfeitosTérmicos,
                Tempo = input.Tempo,
                TipoCritérioRuptura = input.TipoCritérioRuptura
            };

            if (input.PoroElastico != null)
            {
                var inputporo = input.PoroElastico;
                var poro = new PoroelasticoDTO
                {
                    CoeficienteDifusãoSoluto = inputporo.CoeficienteDifusãoSoluto,
                    CoeficienteDissociaçãoSoluto = inputporo.CoeficienteDissociaçãoSoluto,
                    CoeficienteInchamento = inputporo.CoeficienteInchamento,
                    CoeficienteReflexão = inputporo.CoeficienteReflexão,
                    ConcentraçãoSolFluidoPerfuração = inputporo.ConcentraçãoSolFluidoPerfuração,
                    ConcentraçãoSolutoRocha = inputporo.ConcentraçãoSolutoRocha,
                    DensidadeFluidoFormação = inputporo.DensidadeFluidoFormação,
                    DifusidadeTérmica = inputporo.DifusidadeTérmica,
                    ExpansãoTérmicaRocha = inputporo.ExpansãoTérmicaRocha,
                    ExpansãoTérmicaVolumeFluidoPoros = inputporo.ExpansãoTérmicaVolumeFluidoPoros,
                    Kf = inputporo.Kf,
                    Litologias = inputporo.Litologias,
                    MassaMolarSoluto = inputporo.MassaMolarSoluto,
                    PropriedadeTérmicaTemperaturaFormação = inputporo.PropriedadeTérmicaTemperaturaFormação,
                    TemperaturaFormação = inputporo.TemperaturaFormação,
                    TemperaturaPoço = inputporo.TemperaturaPoço,
                    TipoSal = inputporo.TipoSal,
                    Viscosidade = inputporo.Viscosidade,
                    TemperaturaFormaçãoFisicoQuimica = inputporo.TemperaturaFormaçãoFisicoQuimica
                };
                entradas.PoroElastico = poro;
            }
            return entradas;
        }

        private DadosMalha PreencherDadosMalha(ICálculoGradientes input)
        {
            var dados = new DadosMalha(input.Dadosmalha.AnguloDivisao, (int)input.Dadosmalha.AnguloMaxMinIncremento, input.Dadosmalha.AnguloInternoPorExterno, input.Dadosmalha.AnguloTotal);            
            return dados;
        }

        private async Task<IReadOnlyCollection<PerfilBase>> PreencherPerfisEntrada(List<string> ids)
        {
            var perfisEntrada = new List<PerfilBase>();

            for (int i = 0; i < ids.Count; i++)
            {
                var perfilSaida = await _perfilReadOnlyRepository.ObterPerfil(ids[i]);
                perfisEntrada.Add(perfilSaida);
            }

            return perfisEntrada;
        }

        private async Task<IList<PerfilBase>> PreencherListaPerfisSaída(List<string> idPerfis, string nomeCálculo)
        {
            var listaPerfil = new List<PerfilBase>();

            foreach (var perfil in idPerfis)
            {
                var perfilSaida = await _perfilReadOnlyRepository.ObterPerfil(perfil);
                var nomePerfil = perfilSaida.Mnemonico + "_" + nomeCálculo;
                perfilSaida.EditarNome(nomePerfil);
                listaPerfil.Add(perfilSaida);
            }

            return listaPerfil;
        }
    }
    
}
