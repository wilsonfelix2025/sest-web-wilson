using SestWeb.Domain.DTOs;
using SestWeb.Domain.Importadores.SestTR1.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SestWeb.Domain.Importadores.SestTR1.Deep
{
    public class LeitorDeepRegistrosTR1
    {
        public List<RegistroDTO> Registros { get; private set; } = new List<RegistroDTO>();
        public LeitorDeepRegistrosTR1(List<string> linhas)
        {
            for (int i = 0; i < linhas.Count; i++)            
            {
                if (linhas[i].Contains("<GraphicsRegisterOfDrillingPunctualWithMagnitudeItem "))
                {
                    var tipo = LeitorHelperTR1.ObterAtributo(linhas[i], "RegistersOfDrillingType");
                    var tipoRegistro = LerTipoRegistro(tipo);

                    //ignorar alguns tipos de registro
                    if (string.IsNullOrWhiteSpace(tipoRegistro))
                        continue;

                    var registro = new RegistroDTO();
                    registro.Tipo = tipoRegistro;

                    var ponto = new PontoRegistroDTO();
                    ponto.Pm = LeitorHelperTR1.ObterAtributo(linhas[i], "PM");
                    ponto.Valor = LeitorHelperTR1.ObterAtributo(linhas[i + 1], "Value");
                    ponto.Comentário = LeitorHelperTR1.ObterAtributo(linhas[i], "Description");

                    registro.Pontos.Add(ponto);

                    Registros.Add(registro);
                }

                if (linhas[i].Contains("<GraphicsRegisterOfDrillingStretchWithoutMagnitudeItem "))
                {
                    var tipo = LeitorHelperTR1.ObterAtributo(linhas[i], "RegistersOfDrillingType");
                    var tipoRegistro = LerTipoRegistro(tipo);

                    var registro = new RegistroDTO();
                    registro.Tipo = tipoRegistro;

                    //ignorar alguns tipos de registro
                    if (string.IsNullOrWhiteSpace(tipoRegistro))
                        continue;

                    var ponto = new PontoRegistroDTO();

                    //não é evento, não tem topo-base. Ficam dois registros
                    if (tipoRegistro == "Perfilagem")
                    {
                        ponto.Pm = LeitorHelperTR1.ObterAtributo(linhas[i], "PMTop");
                        ponto.Valor = LeitorHelperTR1.ObterAtributo(linhas[i], "PlotValue");
                        ponto.Comentário = LeitorHelperTR1.ObterAtributo(linhas[i], "Description");
                        registro.Pontos.Add(ponto);

                        ponto = new PontoRegistroDTO();
                        ponto.Pm = LeitorHelperTR1.ObterAtributo(linhas[i], "PMBase");
                        ponto.Valor = LeitorHelperTR1.ObterAtributo(linhas[i], "PlotValue");
                        ponto.Comentário = LeitorHelperTR1.ObterAtributo(linhas[i], "Description");
                        registro.Pontos.Add(ponto);
                    }
                    else
                    {

                        ponto.PmTopo = LeitorHelperTR1.ObterAtributo(linhas[i], "PMTop");
                        ponto.PmBase = LeitorHelperTR1.ObterAtributo(linhas[i], "PMBase");
                        ponto.Valor = LeitorHelperTR1.ObterAtributo(linhas[i], "PlotValue");
                        ponto.Comentário = LeitorHelperTR1.ObterAtributo(linhas[i], "Description");

                        registro.Pontos.Add(ponto);
                    }
                    Registros.Add(registro);
                }

                if (linhas[i].Contains("<GraphicsRegisterOfDrillingPunctualWithoutMagnitudeItem "))
                {
                    var tipo = LeitorHelperTR1.ObterAtributo(linhas[i], "RegistersOfDrillingType");
                    var tipoRegistro = LerTipoRegistro(tipo);

                    //ignorar alguns tipos de registro
                    if (string.IsNullOrWhiteSpace(tipoRegistro))
                        continue;

                    var registro = new RegistroDTO();
                    registro.Tipo = tipoRegistro;

                    var ponto = new PontoRegistroDTO();
                    ponto.Pm = LeitorHelperTR1.ObterAtributo(linhas[i], "PM");
                    ponto.Valor = LeitorHelperTR1.ObterAtributo(linhas[i], "PlotValue");
                    ponto.Comentário = LeitorHelperTR1.ObterAtributo(linhas[i], "Description");

                    registro.Pontos.Add(ponto);

                    Registros.Add(registro);
                }
            }

        }

        private string LerTipoRegistro(string tipo)
        {
            var tipoRegistro = string.Empty;
            switch (tipo)
            {
                case "LotWithoutAbsorption":
                    tipoRegistro = "FIT";
                    break;
                case "LotWithAbsorption":
                    tipoRegistro = "LOT";
                    break;
                case "LotWithFracture":
                    tipoRegistro = "Minifrac";
                    break;
                case "Phi":
                    tipoRegistro = "Ângulo de Atrito";
                    break;
                case "k":
                    tipoRegistro = "Permeabilidade";
                    break;
                case "PR":
                    tipoRegistro = "Coeficiente de Poisson";
                    break;
                case "Ks":
                    tipoRegistro = "Módulo de Compressibilidade dos Grãos";
                    break;
                case "UCS":
                    tipoRegistro = "Resistência à Compressão Uniaxial";
                    break;
                case "T0":
                    tipoRegistro = "Resistência à Tração";
                    break;
                case "c":
                    tipoRegistro = "Coesão";
                    break;
                case "E":
                    tipoRegistro = "Módulo de Young";
                    break;
                case "Biot":
                    tipoRegistro = "Coeficiente Biot";
                    break;
                case "Logging":
                    tipoRegistro = "Perfilagem";
                    break;
                case "CuttingsPhotos":
                    tipoRegistro = "Cascalho outros";
                    break;
                case "StuckPipe":
                    tipoRegistro = "Prisão";
                    break;
                case "Stumble":
                    tipoRegistro = "Topada";
                    break;
                case "Fishing":
                    tipoRegistro = "Pescaria";
                    break;
                case "WorkingPotins":
                    tipoRegistro = "Pontos trabalhados";
                    break;
                case "InductedFractures":
                    tipoRegistro = "Fraturas induzidas";
                    break;
                case "Coring":
                    tipoRegistro = "Testemunhagem";
                    break;
                case "Gas":
                    tipoRegistro = "Gás de formação";
                    break;
                case "MudLoss":
                    tipoRegistro = "Perda";
                    break;
                case "MudGain":
                    tipoRegistro = "Ganho";
                    break;
                case "NaturalFractures":
                    tipoRegistro = "Fraturas Naturais";
                    break;
                case "Backreaming":
                    tipoRegistro = "Repasse";
                    break;
                case "Packoff":
                    tipoRegistro = "Empacotamento";
                    break;
                case "RFT":
                case "MDT":
                    tipoRegistro = string.Empty;
                    break;
                default:
                    tipoRegistro = tipo;
                    break;
            }

            return tipoRegistro;
        }
    }
}
