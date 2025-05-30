using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Importadores.Utils;
using SestWeb.Domain.Importadores.SestTR1.Utils;
using System;
using System.Collections.Generic;
using SestWeb.Domain.Entities.PontosEntity;

namespace SestWeb.Domain.Importadores.SestTR1.Deep
{
    public class LeitorDeepPerfilTR1
    {
        public List<PerfilDTO> Perfis { get; private set; } = new List<PerfilDTO>();

        public void AdicionarPerfil(List<string> linhas)
        {
            List<PontoDTO> pontos = new List<PontoDTO>();
            var nomePerfil = "";
            var mnemônico = "";

            foreach (var linha in linhas)
            {
                if (LeitorHelperTR1.ÉDataset(linha))
                {
                    nomePerfil = LeitorHelperTR1.ObterAtributo(linha, "Name");
                    mnemônico = MapearTipoDePerfil(LeitorHelperTR1.ObterNomeDataset(linha));
                }
                else if (LeitorHelperTR1.ÉPontoGenérico(linha))
                {
                    var valorPonto = LeitorHelperTR1.ObterAtributo(linha, "Value");
                    var éNumeroValido = double.TryParse(valorPonto, out var doubleValorPonto);

                    if (éNumeroValido && !ReferênciaParaImportação.ÉSemValor(doubleValorPonto))
                    {
                        pontos.Add(new PontoDTO
                        {
                            Pm = LeitorHelperTR1.ObterAtributo(linha, "Depth"),
                            Valor = valorPonto,
                            Origem = OrigemPonto.Importado
                        });
                    }
                }
            }

            Perfis.Add(new PerfilDTO
            {
                Nome = nomePerfil,
                PontosDTO = pontos,
                Mnemonico = mnemônico
            });
        }

        // TODO abstrair esse código para um utils (está aqui e no leitor shallow)
        private string MapearTipoDePerfil(string tipoPerfil)
        {
            string novoMnemônico;

            switch (tipoPerfil)
            {
                case "Biot":
                    novoMnemônico = "BIOT"; break;
                case "BitSize":
                    novoMnemônico = "DIAM_BROCA"; break;
                case "Caliper":
                    novoMnemônico = "CALIP"; break;
                case "ClayVolume":
                    novoMnemônico = "VCL"; break;
                case "Cohesion":
                    novoMnemônico = "COESA"; break;
                case "Density":
                    novoMnemônico = "RHOB"; break;
                case "IROP":
                    novoMnemônico = "IROP"; break;
                case "ROP":
                    novoMnemônico = "ROP"; break;
                case "RPM":
                    novoMnemônico = "RPM"; break;
                case "WOB":
                    novoMnemônico = "WOB"; break;
                case "DTMC":
                    novoMnemônico = "DTMC"; break;
                case "DTMS":
                    novoMnemônico = "DTMS"; break;
                case "ECD":
                    novoMnemônico = "GECD"; break;
                case "ExponentD":
                    novoMnemônico = "ExpoenteD"; break;
                case "FractureGrad":
                    novoMnemônico = "GQUEBRA"; break;
                case "FrictionAngle":
                    novoMnemônico = "ANGAT"; break;
                case "GammaRay":
                    novoMnemônico = "GRAY"; break;
                case "Generic":
                    novoMnemônico = "GENERICO"; break;
                case "GrainDensity":
                    novoMnemônico = "RHOG"; break;
                case "K0":
                    novoMnemônico = "K0"; break;
                case "Ks":
                    novoMnemônico = "KS"; break;
                case "LowerCollapseGrad":
                    novoMnemônico = "GCOLI"; break;
                case "MaxHorStress":
                    novoMnemônico = "THORmax"; break;
                case "MaxHorStressGrad":
                    novoMnemônico = "GTHORmax"; break;
                case "MinHorStress":
                    novoMnemônico = "THORmin"; break;
                case "MinHorStressAzimuth":
                    novoMnemônico = "AZTHmin"; break;
                case "MinHorStressGrad":
                    novoMnemônico = "GFRAT_σh"; break;
                case "MudWeight":
                    novoMnemônico = "GLAMA"; break;
                case "OverburdenGrad":
                    novoMnemônico = "GSOBR"; break;
                case "Permeability":
                    novoMnemônico = "PERM"; break;
                case "Poisson":
                    novoMnemônico = "POISS"; break;
                case "PorePressure":
                    novoMnemônico = "PPORO"; break;
                case "PorePressureGrad":
                    novoMnemônico = "GPORO"; break;
                case "Porosity":
                    novoMnemônico = "PORO"; break;
                case "Resistivity":
                    novoMnemônico = "RESIST"; break;
                case "RotarySpeed":
                    novoMnemônico = "RPM"; break;
                case "ShearSonic":
                    novoMnemônico = "DTS"; break;
                case "Sonic":
                    novoMnemônico = "DTC"; break;
                case "TensileStrength":
                    novoMnemônico = "RESTR"; break;
                case "THORm_THORM":
                    novoMnemônico = "RET"; break;
                case "UnconfCompressiveStrength":
                    novoMnemônico = "UCS"; break;
                case "UpperCollapseGrad":
                    novoMnemônico = "GCOLS"; break;
                case "VerticalStress":
                    novoMnemônico = "TVERT"; break;
                case "Young":
                    novoMnemônico = "YOUNG"; break;
                default:
                    novoMnemônico = string.Empty; break;
            }

            if (novoMnemônico == string.Empty)
            {
                throw new ArgumentException($"O tipo de perfil não está mapeado para importações do SEST TR1: {tipoPerfil}");
            }

            return TiposPerfil.GeTipoPerfil(novoMnemônico).Mnemônico;
        }
    }
}
