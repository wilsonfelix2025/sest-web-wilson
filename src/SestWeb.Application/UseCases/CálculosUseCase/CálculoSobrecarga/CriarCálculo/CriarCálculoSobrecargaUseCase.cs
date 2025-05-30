using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.GradienteSobrecarga.Factory;
using SestWeb.Domain.Entities.Cálculos.Sobrecarga.Factory;
using SestWeb.Domain.Entities.Cálculos.TensãoVertical.Factory;
using SestWeb.Domain.Entities.Perfis.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoSobrecarga.CriarCálculo
{
    internal class CriarCálculoSobrecargaUseCase : ICriarCálculoSobrecargaUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly ICálculoSobrecargaFactory _cálculoFactory;

        public CriarCálculoSobrecargaUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository,
            IPoçoReadOnlyRepository poçoReadOnlyRepository, ICálculoSobrecargaFactory cálculoFactory)
        {
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _cálculoFactory = cálculoFactory;
        }

        public async Task<CriarCálculoSobrecargaOutput> Execute(CriarCálculoSobrecargaInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoço(input.IdPoço);

                if (poço == null)
                {
                    return CriarCálculoSobrecargaOutput.CálculoSobrecargaNãoCriado("poço não encontrado.");
                }

                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.IdRhob);

                if (perfil == null)
                {
                    return CriarCálculoSobrecargaOutput.CálculoSobrecargaNãoCriado("perfil não encontrado.");
                }

                var perfisEntrada = new List<PerfilBase> { perfil };
                
                var sobrecargaPerfisSaida = new List<PerfilBase>();
                var result = _cálculoFactory.CreateCálculoSobrecarga(input.Nome, "Sobrecarga", perfisEntrada, sobrecargaPerfisSaida, poço.Trajetória,
                    poço.Litologias.FirstOrDefault(), poço.DadosGerais, out var calc);
                calc.Execute();

                await _poçoWriteOnlyRepository.CriarCálculo(poço, (Cálculo)calc, "Sobrecarga");

                return CriarCálculoSobrecargaOutput.CálculoSobrecargaCriado(calc);
            }
            catch (Exception e)
            {
                return CriarCálculoSobrecargaOutput.CálculoSobrecargaNãoCriado(e.Message);
            }
        }
    }
}
