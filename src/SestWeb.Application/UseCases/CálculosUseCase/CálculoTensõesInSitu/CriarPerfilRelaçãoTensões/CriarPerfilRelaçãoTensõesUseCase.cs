using SestWeb.Application.Repositories;
using SestWeb.Application.Validadores;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Poço;
using System;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.PontosEntity;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CriarPerfilRelaçãoTensões
{
    public class CriarPerfilRelaçãoTensõesUseCase : ICriarPerfilRelaçãoTensõesUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public CriarPerfilRelaçãoTensõesUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPerfilWriteOnlyRepository perfilWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<CriarPerfilRelaçãoTensõesOutput> Execute(CriarPerfilRelaçãoInput input)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(input.IdPoço);
                if (poço == null)
                    return CriarPerfilRelaçãoTensõesOutput.PerfilNãoCriado("Poço não encontrado");

                //validção dos irmãos
                var validator = new CriarPerfilRelaçãoTensõesValidator(_perfilReadOnlyRepository, poço);

                var resultUseCase = validator.Validate(input);

                if (resultUseCase.IsValid)
                {
                    var perfil = CriarPerfil(input, poço);
                    await _perfilWriteOnlyRepository.CriarPerfil(poço.Id, perfil, poço);
                    return CriarPerfilRelaçãoTensõesOutput.PerfilCriado(perfil);
                }

                return CriarPerfilRelaçãoTensõesOutput.PerfilNãoCriado(string.Join(";\n", resultUseCase.Errors));
            }
            catch (Exception e)
            {
                return CriarPerfilRelaçãoTensõesOutput.PerfilNãoCriado(e.Message);
            }
        }

        private PerfilBase CriarPerfil(CriarPerfilRelaçãoInput input, Poço poço)
        {
            var perfil = PerfisFactory.Create("RET", input.NomePerfil, poço.Trajetória, poço.ObterLitologiaPadrão());

            foreach (var item in input.Valores)
            {
                perfil.AddPontoEmPm(poço.Trajetória, item.PMTopo, item.Valor, TipoProfundidade.PM, OrigemPonto.Importado);
                perfil.AddPontoEmPm(poço.Trajetória, item.PMBase, item.Valor, TipoProfundidade.PM, OrigemPonto.Importado);
            }

            return perfil;
        }
    }
}
