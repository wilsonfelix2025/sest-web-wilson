using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using SestWeb.Application.Validadores;
using SestWeb.Domain.DTOs.Cálculo.Gradientes;
using SestWeb.Domain.Entities.Cálculos.Base;
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

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.EditarCálculo
{
    public class EditarCálculoGradientesUseCase : IEditarCálculoGradientesUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoGradientesFactory _cálculoFactory;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;
        private readonly IPipelineUseCase _pipeUseCase;


        public EditarCálculoGradientesUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, ICálculoGradientesFactory cálculoFactory
            , ICorrelaçãoReadOnlyRepository correlaçãoReadOnlyRepository, ICorrelaçãoFactory correlaçãoFactory
            , IPipelineUseCase pipeUseCase)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _cálculoFactory = cálculoFactory;
            _correlaçãoReadOnlyRepository = correlaçãoReadOnlyRepository;
            _correlaçãoFactory = correlaçãoFactory;
            _pipeUseCase = pipeUseCase;
        }

        public async Task<EditarCálculoGradientesOutput> Execute(EditarCálculoGradientesInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return EditarCálculoGradientesOutput.CálculoNãoEditado("Poço não encontrado");

                var cálculo = (Domain.Entities.Cálculos.Gradientes.CálculoGradientes)poço.Cálculos.First(c => c.Id.ToString() == input.IdCálculo);

                ////preparação para as entradas para a factory
                var listaPerfisEntrada = await PreencherPerfisEntrada(input);
                var listaPerfisSaída = await PreencherListaPerfisSaída(cálculo.PerfisSaída.IdPerfis, input.NomeCálculo);
                var dadosMalha = PreencherDadosMalha(input);
                var entradasColapsos = PreencherEntradasColapsos(input);

                ////retorna se a entidade calculo criada está valida ou não
                var result = _cálculoFactory.CreateCálculoGradientes(input.NomeCálculo, "Gradientes"
                    , listaPerfisEntrada.ToList(), listaPerfisSaída, poço.Trajetória, poço.ObterLitologiaPadrão()
                    , dadosMalha, entradasColapsos, out var calc);

                ////se for valida, executa o calculo
                if (result.IsValid)
                    calc.Execute();
                else
                    return EditarCálculoGradientesOutput.CálculoNãoEditado(string.Join(";\n", result.Errors));

                //validção dos irmãos
                var validator = new CálculoValidator(_poçoReadOnlyRepository, poço);
                var resultUseCase = validator.Validate((Cálculo)calc);

                if (resultUseCase.IsValid)
                {
                    await _poçoWriteOnlyRepository.EditarCálculo(poço, (Cálculo)calc, input.IdCálculo, "Gradientes");

                    var perfisAlterados = await _pipeUseCase.Execute(poço, calc, input.IdCálculo);

                    return EditarCálculoGradientesOutput.CálculoEditado(calc, perfisAlterados);
                }

                return EditarCálculoGradientesOutput.CálculoNãoEditado(string.Join(";\n", result.Errors));
            }
            catch (Exception e)
            {
                return EditarCálculoGradientesOutput.CálculoNãoEditado(e.Message);
            }
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

        private EntradasColapsosDTO PreencherEntradasColapsos(EditarCálculoGradientesInput input)
        {            
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

            if (input.TipoModelo == ModeloAnáliseGradientesEnum.Poroelástico)
            {
                var inputporo = input.ParâmetrosPoroElástico;
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

        private DadosMalha PreencherDadosMalha(EditarCálculoGradientesInput input)
        {
            var dados = new DadosMalha(input.MalhaNunDivAng, input.MalhaNunDivRad, input.MalhaRextRint, input.MalhaRMaxRMin);
            return dados;
        }

        private async Task<IReadOnlyCollection<PerfilBase>> PreencherPerfisEntrada(EditarCálculoGradientesInput input)
        {
            var perfisEntrada = new List<PerfilBase>();

            if (!string.IsNullOrEmpty(input.PerfilYOUNGId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilYOUNGId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilUCSId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilUCSId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilTHORmaxId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilTHORmaxId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilRETRId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilRETRId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilPOROId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilPOROId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilPOISSId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilPOISSId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilPERMId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilPERMId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilKSId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilKSId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilGSOBRId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilGSOBRId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilDIAMBROCAId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilDIAMBROCAId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilCOESAId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilCOESAId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilBIOTId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilBIOTId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilGPOROId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilGPOROId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilANGATId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilANGATId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilAZTHminId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilAZTHminId);
                perfisEntrada.Add(perfil);
            }

            if (!string.IsNullOrEmpty(input.PerfilTHORminId))
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.PerfilTHORminId);
                perfisEntrada.Add(perfil);
            }

            return perfisEntrada;
        }

    }

}
