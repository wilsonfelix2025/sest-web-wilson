using MathNet.Numerics;
using SestWeb.Application.Repositories;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.ConversãoGradienteTensão
{
    public class ConversãoGradienteTensãoUseCase : IConversãoGradienteTensãoUseCase
    {
        private readonly IPerfilReadOnlyRepository _perfilReadOnlyRepository;
        private readonly IPerfilWriteOnlyRepository _perfilWriteOnlyRepository;
        private readonly IPoçoReadOnlyRepository _poçoReadOnlyRepository;

        public ConversãoGradienteTensãoUseCase(IPerfilReadOnlyRepository perfilReadOnlyRepository, IPerfilWriteOnlyRepository perfilWriteOnlyRepository
            , IPoçoReadOnlyRepository poçoReadOnlyRepository)
        {
            _perfilWriteOnlyRepository = perfilWriteOnlyRepository;
            _perfilReadOnlyRepository = perfilReadOnlyRepository;
            _poçoReadOnlyRepository = poçoReadOnlyRepository;
        }

        public async Task<ConversãoOutput> Execute(ConversãoInput input)
        {
            try
            {
                var perfil = await _perfilReadOnlyRepository.ObterPerfil(input.IdPerfil);
                if (perfil == null)
                    return ConversãoOutput.NãoConvertido("Perfil não encontrado");

                if (input.TipoConversão == TipoConversãoEnum.Gradiente && perfil.PodeSerConvertidoParaGradiente == false)
                    return ConversãoOutput.NãoConvertido("Conversão não permitida");

                if ((input.TipoConversão == TipoConversãoEnum.PressãoAbsoluta || input.TipoConversão == TipoConversãoEnum.PressãoManométrica) && perfil.PodeSerConvertidoParaTensão == false)
                    return ConversãoOutput.NãoConvertido("Conversão não permitida");


                var poço = await _poçoReadOnlyRepository.ObterPoçoSemPerfis(perfil.IdPoço);
                var pontos = perfil.GetPontos();
                var mnemônico = PreencherMnemônico(perfil.Mnemonico);
                var nome = PreencherNome(mnemônico, perfil.Nome, input.TipoConversão);
                nome = await VerificaNome(nome, poço.Id);

                var novoPerfil = PerfisFactory.Create(mnemônico, nome, poço.Trajetória, poço.ObterLitologiaPadrão());
                var interpolator = Interpolate.Linear(pontos.Select(x => x.Pv.Valor), pontos.Select(x => x.Valor));

                var pv = 0.0;

                for (int i = 0; i < pontos.Count; i++)
                {
                    if (i == 0)
                    {
                        novoPerfil.AddPontoEmPv(poço.Trajetória, pontos[i].Pv, PreencherValor(input.TipoConversão, pontos[i].Pv.Valor, pontos[i].Valor), TipoProfundidade.PV, OrigemPonto.Calculado);
                        pv = pontos[i].Pv.Valor;
                    }
                    else if (pv < pontos[i].Pv.Valor)
                    {
                        while (pv < pontos[i].Pv.Valor && pontos[i].Pv.Valor - pv > 1)
                        {
                            var valor = interpolator.Interpolate(pv);
                            novoPerfil.AddPontoEmPv(poço.Trajetória, pv, PreencherValor(input.TipoConversão, pv, valor), TipoProfundidade.PV, OrigemPonto.Interpolado);
                            pv = pv + 1;
                        }

                        novoPerfil.AddPontoEmPv(poço.Trajetória, pontos[i].Pv, PreencherValor(input.TipoConversão, pontos[i].Pv.Valor, pontos[i].Valor), TipoProfundidade.PV, OrigemPonto.Calculado);
                        pv = pontos[i].Pv.Valor;
                    }
                    else
                    {
                        novoPerfil.AddPontoEmPv(poço.Trajetória, pontos[i].Pv, PreencherValor(input.TipoConversão, pontos[i].Pv.Valor, pontos[i].Valor), TipoProfundidade.PV, OrigemPonto.Calculado);
                        pv = pontos[i].Pv.Valor;
                    }

                    pv = pv + 1;

                }

                await _perfilWriteOnlyRepository.CriarPerfil(poço.Id, novoPerfil, poço);

                return ConversãoOutput.Convertido(novoPerfil);

            }
            catch (Exception e)
            {
                return ConversãoOutput.NãoConvertido(e.Message);
            }
        }

        private async Task<string> VerificaNome(string nomeOriginal, string poçoId)
        {
            var contador = 1;
            var existeNome = await _perfilReadOnlyRepository.ExistePerfilComMesmoNome(nomeOriginal, poçoId);
            var nome = nomeOriginal;

            while (existeNome)
            {
                nome = nomeOriginal + "_" + contador.ToString();
                contador++;
                existeNome = await _perfilReadOnlyRepository.ExistePerfilComMesmoNome(nome, poçoId);
            }

            return nome;

        }

        private double PreencherValor(TipoConversãoEnum tipoConversão, double pv, double valorPonto)
        {
            var valor = 0.0;
            var fatorConversão = 5.8674;
            var fatorManométrico = 14.69595;

            if (tipoConversão == TipoConversãoEnum.PressãoAbsoluta)
                valor = valorPonto * (pv / fatorConversão);
            else if (tipoConversão == TipoConversãoEnum.PressãoManométrica)
                valor = (valorPonto * (pv / fatorConversão)) - fatorManométrico;
            else if (tipoConversão == TipoConversãoEnum.Gradiente)
                valor = valorPonto * (fatorConversão / pv);

            return valor;
        }

        private string PreencherNome(string mnemônico, string nomePerfil, TipoConversãoEnum tipoConversão)
        {
            var nome = mnemônico + "_de_" + nomePerfil;
            if (tipoConversão == TipoConversãoEnum.PressãoManométrica)
                nome = nome + "_gauge";

            return nome;
        }

        private string PreencherMnemônico(string mnemonico)
        {
            switch (mnemonico)
            {
                case "GTHORmax":
                    return "THORmax";
                case "GFRAT_σh":
                    return "THORmin";
                case "GCOLI":
                    return "PCOLI";
                case "GCOLS":
                    return "PCOLS";
                case "GFRAT":
                    return "THORmin";
                case "GPORO":
                case "GPPI":
                    return "PPORO";
                case "GQUEBRA":
                    return "PQUEBRA";
                case "GSOBR":
                    return "TVERT";
                case "THORmax":
                    return "GTHORmax";
                case "THORmin":
                    return "GFRAT_σh";
                case "PCOLI":
                    return "GCOLI";
                case "PCOLS":
                    return "GCOLS";
                case "PPORO":
                    return "GPORO";
                case "PQUEBRA":
                    return "GQUEBRA";
                case "TVERT":
                    return "GSOBR";
                default:
                    return "";
            }
        }
    }
}
