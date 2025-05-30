using System;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Enums;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterQtdPontosPerfil
{
    internal class ObterQtdPontosPerfilUseCase : IObterQtdPontosPerfilUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ObterQtdPontosPerfilUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ObterQtdPontosPerfilOutput> Execute(string idPerfil, ObterQtdPontosPerfilInput input)
        {
            try
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(idPerfil);
                var qtd = 0;

                if (perfil == null)
                    return ObterQtdPontosPerfilOutput.QtdNãoObtida($"Não foi possível encontrar perfil com id {idPerfil}.");

                if (input.LitologiasSelecionadas != null && input.LitologiasSelecionadas.Any() && input.LitologiasSelecionadas.FirstOrDefault() != null)
                {

                    qtd = input.TipoTrecho == TipoDeTrechoEnum.Inicial
                        ? perfil.GetPontos().Count(p => p.Pm.Valor <= input.PmLimite && p.TipoRocha != null &&
                                                                  input.LitologiasSelecionadas.Contains(p.TipoRocha.Mnemonico))
                        : perfil.GetPontos().Count(p => p.Pm.Valor >= input.PmLimite && p.TipoRocha != null &&
                                                                  input.LitologiasSelecionadas.Contains(p.TipoRocha.Mnemonico));
                }
                else
                {
                    qtd = input.TipoTrecho == TipoDeTrechoEnum.Inicial
                        ? perfil.GetPontos().Count(p => p.Pm.Valor <= input.PmLimite)
                        : perfil.GetPontos().Count(p => p.Pm.Valor >= input.PmLimite);
                }

                return ObterQtdPontosPerfilOutput.QtdObtida(qtd);
            }
            catch (Exception e)
            {
                return ObterQtdPontosPerfilOutput.QtdNãoObtida(e.Message);
            }
        }

    }
}
