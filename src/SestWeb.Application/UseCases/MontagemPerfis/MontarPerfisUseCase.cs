using SestWeb.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.MontagemPerfis;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Application.UseCases.MontagemPerfis
{
    public class MontarPerfisUseCase : IMontarPerfisUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;
        private readonly IPoçoWriteOnlyRepository _poçoWriteOnlyRepository;

        public MontarPerfisUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPerfilWriteOnlyRepository perfilWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository, IPoçoWriteOnlyRepository poçoWriteOnlyRepository)
        {
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
            _poçoWriteOnlyRepository = poçoWriteOnlyRepository;
        }

        public async Task<MontarPerfisOutput> Execute(MontarPerfisInput input, string idPoço)
        {
            try
            {
                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(idPoço);
                if (poço == null)
                    return MontarPerfisOutput.MontarPerfisComFalhaDeValidação("Poço não encontrado.");


                var novosPerfis = new List<PerfilBase>();
                var lito = poço.ObterLitologiaPadrão();

                var perfisCorrelação = await PreencherPerfisCorrelação(input);
                var perfis = await PreencherPerfis(input, poço.Trajetória, poço.ObterLitologiaPadrão());
                var litologiaVazia = false;
                var nomePoçoLitologiaVazia = "";
                foreach (var dado in input.Lista)
                {
                    var poçoCorrelação = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(dado.IdPoçoApoio);

                    if (poçoCorrelação.Litologias.Count == 0)
                    {
                        litologiaVazia = true;
                        if (!nomePoçoLitologiaVazia.Contains(poçoCorrelação.Nome))
                            nomePoçoLitologiaVazia = nomePoçoLitologiaVazia + poçoCorrelação.Nome + ", ";
                    }

                    var perfisCorrelaçãoPoço = perfisCorrelação.Where(p => p.IdPoço == poçoCorrelação.Id).ToList();
                    var montadorDePerfil = new MontadorDePerfil(poçoCorrelação, poço, perfisCorrelaçãoPoço, perfis,
                        (dado.PvTopoApoio * -1) + poçoCorrelação.DadosGerais.Geometria.MesaRotativa
                        , (dado.PvBaseApoio * -1) + poçoCorrelação.DadosGerais.Geometria.MesaRotativa
                        , (dado.PvTopo * -1) + poço.DadosGerais.Geometria.MesaRotativa
                        , (dado.PvBase * -1) + poço.DadosGerais.Geometria.MesaRotativa, input.RemoverTendência);
                    var montadorDeLitologia = new MontadorDeLitologia(poçoCorrelação, poço
                        , (dado.PvTopoApoio * -1) + poçoCorrelação.DadosGerais.Geometria.MesaRotativa
                        , (dado.PvBaseApoio * -1) + poçoCorrelação.DadosGerais.Geometria.MesaRotativa
                        , (dado.PvTopo * -1) + poço.DadosGerais.Geometria.MesaRotativa
                        , (dado.PvBase * -1) + poço.DadosGerais.Geometria.MesaRotativa);
                    lito = montadorDeLitologia.MontarLitologia(litologiaVazia);
                    novosPerfis = montadorDePerfil.MontarPerfis();
                }

                lito.CompletarTrechosSenoidais(poço.Trajetória);

                if (novosPerfis.Any())
                {
                    foreach (var novoPerfil in novosPerfis)
                    {
                        await _perfilWriteOnlyRepository.CriarPerfil(poço.Id, novoPerfil, poço);
                    }
                }

                if (lito != null && lito.ContémPontos())
                {
                    await _poçoWriteOnlyRepository.AtualizarLitologias(poço.Id, poço);
                }
                else
                {
                    lito = poço.ObterLitologiaPadrão();
                }

                return MontarPerfisOutput.MontarPerfisComSucesso(novosPerfis, lito);

            }
            catch (Exception e)
            {
                return MontarPerfisOutput.MontarPerfisComFalha(e.Message);
            }

        }

        private async Task<IList<PerfilBase>> PreencherPerfisCorrelação(MontarPerfisInput input)
        {
            var lista = new List<PerfilBase>();

            foreach (var item in input.Lista)
            {
                if (!string.IsNullOrWhiteSpace(item.IdResistApoio))
                {
                    var perfilResist = await _perfilReadOnlyRepository.ObterPerfil(item.IdResistApoio);
                    lista.Add(perfilResist);
                }

                if (!string.IsNullOrWhiteSpace(item.IdDTSApoio))
                {
                    var perfilDTS = await _perfilReadOnlyRepository.ObterPerfil(item.IdDTSApoio);
                    lista.Add(perfilDTS);
                }

                if (!string.IsNullOrWhiteSpace(item.IdGRayApoio))
                {
                    var perfilGray = await _perfilReadOnlyRepository.ObterPerfil(item.IdGRayApoio);
                    lista.Add(perfilGray);
                }

                if (!string.IsNullOrWhiteSpace(item.IdRhobApoio))
                {
                    var perfilRhob = await _perfilReadOnlyRepository.ObterPerfil(item.IdRhobApoio);
                    lista.Add(perfilRhob);
                }

                if (!string.IsNullOrWhiteSpace(item.IdDTCApoio))
                {
                    var perfilDTC = await _perfilReadOnlyRepository.ObterPerfil(item.IdDTCApoio);
                    lista.Add(perfilDTC);
                }

                if (!string.IsNullOrWhiteSpace(item.IdNPhiApoio))
                {
                    var perfilNPhi = await _perfilReadOnlyRepository.ObterPerfil(item.IdNPhiApoio);
                    lista.Add(perfilNPhi);
                }
            }

            return lista;
        }

        private async Task<List<PerfilBase>> PreencherPerfis(MontarPerfisInput inputMontagem, IConversorProfundidade trajetória, Litologia litologia)
        {
            var lista = new List<PerfilBase>();

            if (!string.IsNullOrWhiteSpace(inputMontagem.NomeResist))
            {
                var perfilResist = PerfisFactory.Create("RESIST", inputMontagem.NomeResist, trajetória, litologia);
                lista.Add(perfilResist);
            }

            if (!string.IsNullOrWhiteSpace(inputMontagem.NomeDTS))
            {
                var perfilDTS = PerfisFactory.Create("DTS", inputMontagem.NomeDTS, trajetória, litologia);
                lista.Add(perfilDTS);
            }

            if (!string.IsNullOrWhiteSpace(inputMontagem.NomeGRay))
            {
                var perfilGray = PerfisFactory.Create("GRAY", inputMontagem.NomeGRay, trajetória, litologia);
                lista.Add(perfilGray);
            }

            if (!string.IsNullOrWhiteSpace(inputMontagem.NomeRhob))
            {
                var perfilRhob = PerfisFactory.Create("RHOB", inputMontagem.NomeRhob, trajetória, litologia);
                lista.Add(perfilRhob);
            }

            if (!string.IsNullOrWhiteSpace(inputMontagem.NomeDTC))
            {
                var perfilDTC = PerfisFactory.Create("DTC", inputMontagem.NomeDTC, trajetória, litologia);
                lista.Add(perfilDTC);
            }

            if (!string.IsNullOrWhiteSpace(inputMontagem.NomeNPhi))
            {
                var perfilNPhi = PerfisFactory.Create("PORO", inputMontagem.NomeNPhi, trajetória, litologia);
                lista.Add(perfilNPhi);
            }


            return lista;
        }
    }
}
