using SestWeb.Application.Repositories;
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

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoGradientes.CriarCálculo
{
    public class CriarCálculoGradientesUseCase : ICriarCálculoGradientesUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoGradientesFactory _cálculoFactory;
        private readonly ICorrelaçãoReadOnlyRepository _correlaçãoReadOnlyRepository;
        private readonly ICorrelaçãoFactory _correlaçãoFactory;

        public CriarCálculoGradientesUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository
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

        public async Task<CriarCálculoGradientesOutput> Execute(CriarCálculoGradientesInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return CriarCálculoGradientesOutput.CálculoNãoCriado("Poço não encontrado");

                ////preparação para as entradas para a factory
                var listaPerfisEntrada = await PreencherPerfisEntrada(input);
                var listaPerfisSaída = PreencherListaPerfisSaída(input.NomeCálculo, poço);
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
                    return CriarCálculoGradientesOutput.CálculoNãoCriado(string.Join(";\n", result.Errors));

                //validção dos irmãos
                var validator = new CálculoValidator(_poçoReadOnlyRepository, poço);
                var resultUseCase = validator.Validate((Cálculo)calc);

                if (resultUseCase.IsValid)
                {
                    await _poçoWriteOnlyRepository.CriarCálculo(poço, (Cálculo)calc, "Gradientes");
                    return CriarCálculoGradientesOutput.CálculoCriado(calc);
                }

                return CriarCálculoGradientesOutput.CálculoNãoCriado(string.Join(";\n", result.Errors));
            }
            catch (Exception e)
            {
                return CriarCálculoGradientesOutput.CálculoNãoCriado(e.Message);
            }
        }

        private EntradasColapsosDTO PreencherEntradasColapsos(CriarCálculoGradientesInput input)
        {            
            var entradas = new EntradasColapsosDTO {
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

        private DadosMalha PreencherDadosMalha(CriarCálculoGradientesInput input)
        {
            var dados = new DadosMalha(input.MalhaNunDivAng, input.MalhaNunDivRad, input.MalhaRextRint, input.MalhaRMaxRMin);            
            return dados;
        }

        private async Task<IReadOnlyCollection<PerfilBase>> PreencherPerfisEntrada(CriarCálculoGradientesInput input)
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

        private List<PerfilBase> PreencherListaPerfisSaída(string nomeCálculo, Poço poço)
        {
            var listaPerfil = new List<PerfilBase>();

            var perfilGQUEBRA = PerfisFactory.Create("GQUEBRA", "GQUEBRA_" + nomeCálculo, poço.Trajetória, poço.ObterLitologiaPadrão());
            var perfilGCOLS = PerfisFactory.Create("GCOLS", "GCOLS_" + nomeCálculo, poço.Trajetória, poço.ObterLitologiaPadrão());
            var perfilGCOLI = PerfisFactory.Create("GCOLI", "GCOLI_" + nomeCálculo, poço.Trajetória, poço.ObterLitologiaPadrão());

            listaPerfil.Add(perfilGQUEBRA);
            listaPerfil.Add(perfilGCOLS);
            listaPerfil.Add(perfilGCOLI);

            return listaPerfil;
        }
    }
    
}
