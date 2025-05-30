using System;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Trajetoria
{
    internal static class GeradorPontosTrajetória
    {
        internal static void GerarDadosPontoTrajetória(PontoDeTrajetória pontoAnterior, PontoDeTrajetória pontoAtual, MétodoDeCálculoDaTrajetória método)
        {
            if (método == MétodoDeCálculoDaTrajetória.RaioCurvatura)
            {
                if (Math.Abs(pontoAnterior.Inclinação - pontoAtual.Inclinação) < 0.009)
                {
                    if (Math.Abs(pontoAnterior.Azimute - pontoAtual.Azimute) < 0.009)
                    {
                        Case4Compute(pontoAnterior, ref pontoAtual);
                    }
                    else
                    {
                        Case3Compute(pontoAnterior, ref pontoAtual);
                    }
                }
                else
                {
                    if (Math.Abs(pontoAnterior.Azimute - pontoAtual.Azimute) < 0.009)
                    {
                        Case2Compute(pontoAnterior, ref pontoAtual);
                    }
                    else
                    {
                        Case1Compute(pontoAnterior, ref pontoAtual);
                    }
                }
            }
            else
            {
                Case5Compute(pontoAnterior, ref pontoAtual);
            }
        }

        internal static void ObterDadosDoPontoPorPm(Profundidade pm, SessãoTrajetória sessão, MétodoDeCálculoDaTrajetória método, out double pv, out double inclinação, out double azimute, out double ns, out double ew, out double dls, out double polCoordDispl, out double polCoordDirec)
        {
            if (método == MétodoDeCálculoDaTrajetória.RaioCurvatura)
            {
                if (Math.Abs(sessão.PontoInicial.Inclinação - sessão.PontoFinal.Inclinação) < 0.009)
                {
                    if (Math.Abs(sessão.PontoInicial.Azimute - sessão.PontoFinal.Azimute) < 0.009)
                    {
                        Case4PmToParameters(pm.Valor, sessão.PontoInicial, sessão.PontoFinal, out pv, out inclinação,
                            out azimute, out ns, out ew, out dls, out polCoordDispl, out polCoordDirec);
                    }
                    else
                    {
                        Case3PmToParameters(pm.Valor, sessão.PontoInicial, sessão.PontoFinal, out pv, out inclinação,
                            out azimute, out ns, out ew, out dls, out polCoordDispl, out polCoordDirec);
                    }
                }
                else
                {
                    if (Math.Abs(sessão.PontoInicial.Azimute - sessão.PontoFinal.Azimute) < 0.009)
                    {
                        Case2PmToParameters(pm.Valor, sessão.PontoInicial, sessão.PontoFinal, out pv, out inclinação,
                            out azimute, out ns, out ew, out dls, out polCoordDispl, out polCoordDirec);
                    }
                    else
                    {
                        Case1PmToParameters(pm.Valor, sessão.PontoInicial, sessão.PontoFinal, out pv, out inclinação,
                            out azimute, out ns, out ew, out dls, out polCoordDispl, out polCoordDirec);
                    }
                }
            }
            else
            {
                Case5PMToParameters(pm.Valor, sessão.PontoInicial, sessão.PontoFinal, out pv, out inclinação,
                    out azimute, out ns, out ew, out dls, out polCoordDispl, out polCoordDirec);
            }
        }

        internal static bool ObterDadosDoPontoPorPv(Profundidade pv, SessãoTrajetória sessão, MétodoDeCálculoDaTrajetória método, out double pm, out double inclinação, out double azimute, out double ns, out double ew, out double dls, out double polCoordDispl, out double polCoordDirec)
        {
            if (método == MétodoDeCálculoDaTrajetória.RaioCurvatura)
            {
                if (Math.Abs(sessão.PontoInicial.Inclinação - sessão.PontoFinal.Inclinação) < 0.009) //inclinação dos pontos é igual
                {
                    if (Math.Abs(sessão.PontoInicial.Azimute - sessão.PontoFinal.Azimute) < 0.009)
                    {
                        return Case4PvToParameters(pv.Valor, sessão.PontoInicial, sessão.PontoFinal, out pm, out inclinação,
                            out azimute, out ns, out ew, out dls, out polCoordDispl, out polCoordDirec);
                    }

                    else
                    {
                        return Case3PvToParameters(pv.Valor, sessão.PontoInicial, sessão.PontoFinal, out pm, out inclinação,
                            out azimute, out ns, out ew, out dls, out polCoordDispl, out polCoordDirec);
                    }
                }
                else //inclinação dos pontos NÂO é igual
                {
                    if (Math.Abs(sessão.PontoInicial.Inclinação - sessão.PontoFinal.Inclinação) < 0.009)
                    {
                        return Case2PvToParameters(pv.Valor, sessão.PontoInicial, sessão.PontoFinal, out pm, out inclinação,
                            out azimute, out ns, out ew, out dls, out polCoordDispl, out polCoordDirec);
                    }
                    else
                    {
                        return Case1PvToParameters(pv.Valor, sessão.PontoInicial, sessão.PontoFinal, out pm, out inclinação,
                            out azimute, out ns, out ew, out dls, out polCoordDispl, out polCoordDirec);
                    }
                }
            }
            else
            {
                return Case5PvToParameters(pv.Valor, sessão.PontoInicial, sessão.PontoFinal, out pm, out inclinação,
                    out azimute, out ns, out ew, out dls, out polCoordDispl, out polCoordDirec);
            }
        }

        //**Case 1 to 4 - Radius of Curvature** 
        // incl1 != incl2, azim1 != azim2
        private static void Case1Compute(PontoDeTrajetória tp1, ref PontoDeTrajetória tp2)
        {
            const double c = Math.PI / 180;

            // Initial point measured depth (well path length)
            var l1 = tp1.Pm.Valor;

            // Final point measured depth (well path length)
            var l2 = tp2.Pm.Valor;

            // Initial point Inclinação
            var incl1 = tp1.Inclinação * c;

            // Final point Inclinação
            var incl2 = tp2.Inclinação * c;

            // Initial point Azimute
            var azim1 = tp1.Azimute * c;

            // Final point Azimute
            var azim2 = tp2.Azimute * c;

            // Check angle diference
            var dAzim = azim2 - azim1;

            if (Math.Abs(dAzim) > Math.PI)
            {
                if (azim2 > azim1)
                {
                    if (azim2 > 0)
                    {
                        azim2 -= 2 * Math.PI;
                    }
                    else
                    {
                        azim1 += 2 * Math.PI;
                    }
                }
                else
                {
                    if (azim1 > 0)
                    {
                        azim1 -= 2 * Math.PI;
                    }
                    else
                    {
                        azim2 += 2 * Math.PI;
                    }
                }
            }

            // Radius-of-Curvature in the vertical plane
            var rv = (l2 - l1) / (incl2 - incl1);

            // Vertical increment
            var dZ = rv * (Math.Sin(incl2) - Math.Sin(incl1));

            // Horizontal increment
            var dH = rv * (Math.Cos(incl1) - Math.Cos(incl2));

            // Radius-of-Curvature in the horizontal plane
            var rh = dH / dAzim;

            // North/South course coordinate
            var dNs = rh * (Math.Sin(azim2) - Math.Sin(azim1));

            // East/West course coordinate
            var dEw = rh * (Math.Cos(azim1) - Math.Cos(azim2));

            // Dog leg severity
            const double dls = 0;

            // Return parameters
            tp2.Pv = new Profundidade(tp1.Pv.Valor + dZ);
            tp2.EW = tp1.EW + dEw;
            tp2.NS = tp1.NS + dNs;
            tp2.DLS = dls;
            tp2.PolCoordDispl = Math.Sqrt(Math.Pow(tp2.NS, 2) + Math.Pow(tp2.EW, 2));
            tp2.PolCoordDirec = Math.Atan2(tp2.EW, tp2.NS) / c;
        }

        private static bool Case1PmToParameters(double pm, PontoDeTrajetória tp1, PontoDeTrajetória tp2, out double pv, out double inclinação, out double azimute, out double ns, out double ew, out double dls, out double polCoordDispl, out double polCoordDirec)
        {
            pv = 0;
            inclinação = 0;
            azimute = 0;
            ns = 0;
            ew = 0;
            dls = 0;
            polCoordDispl = 0;
            polCoordDirec = 0;

            // Obtain measured depths
            var lx = pm;
            var l1 = tp1.Pm.Valor;
            var l2 = tp2.Pm.Valor;

            const double c = Math.PI / 180.0;

            var incl1 = tp1.Inclinação * c;
            var incl2 = tp2.Inclinação * c;

            var azim1 = tp1.Azimute * c;
            var azim2 = tp2.Azimute * c;

            // Check angle diference
            var dAzim = azim2 - azim1;

            if (Math.Abs(dAzim) > Math.PI)
            {
                if (azim2 > azim1)
                {
                    if (azim2 > 0.0)
                    {
                        azim2 -= 2.0 * Math.PI;
                    }
                    else
                    {
                        azim1 += 2.0 * Math.PI;
                    }
                }
                else
                {
                    if (azim1 > 0.0)
                    {
                        azim1 -= 2.0 * Math.PI;
                    }
                    else
                    {
                        azim2 += 2.0 * Math.PI;
                    }
                }
            }

            // Obtain Inclinação of the required point
            var inclx = incl1 + (lx - l1) * (incl2 - incl1) / (l2 - l1);

            // Rv = Radius in the vertical plane (constant in the segment)
            var rv = (l2 - l1) / (incl2 - incl1);

            // Vertical increment
            var dz = rv * (Math.Sin(inclx) - Math.Sin(incl1));

            // Rh = Radius in the horizontal plane (constant in the segment)
            var rh = rv * (Math.Cos(incl1) - Math.Cos(incl2)) / (azim2 - azim1);

            // Prevents overflow
            if (rh.Equals(0.0))
            {
                return false;
            }

            // Obtain Azimute of the required point
            var azimx = azim1 + rv / rh * (Math.Cos(incl1) - Math.Cos(inclx));

            // North/South course coordinate
            var dns = rh * (Math.Sin(azimx) - Math.Sin(azim1));

            // East/West course coordinate
            var dew = rh * (Math.Cos(azim1) - Math.Cos(azimx));

            // Dog leg severity
            dls = 0.0;

            // Return parameters
            pv = tp1.Pv.Valor + dz;
            inclinação = inclx / c;
            azimute = azimx / c;
            ns = tp1.NS + dns;
            ew = tp1.EW + dew;
            polCoordDispl = Math.Sqrt(Math.Pow(ns, 2) + Math.Pow(ew, 2));
            polCoordDirec = Math.Atan2(ew, ns) / c;

            return true;
        }

        private static bool Case1PvToParameters(double pv, PontoDeTrajetória tp1, PontoDeTrajetória tp2, out double pm, out double inclinação, out double azimute, out double ns, out double ew, out double dls, out double polCoordDispl, out double polCoordDirec)
        {
            pm = 0;
            inclinação = 0;
            azimute = 0;
            ns = 0;
            ew = 0;
            dls = 0;
            polCoordDispl = 0;
            polCoordDirec = 0;

            // Obtain vertical depth
            var zx = pv;
            var z1 = tp1.Pv.Valor;
            var z2 = tp2.Pv.Valor;

            var l1 = tp1.Pm.Valor;
            var l2 = tp2.Pm.Valor;

            const double c = Math.PI / 180;

            // Initial point Inclinação
            var incl1 = tp1.Inclinação * c;

            // Final point Inclinação
            var incli2 = tp2.Inclinação * c;

            // Initial point Azimute
            var azim1 = tp1.Azimute * c;

            // Final point Azimute
            var azim2 = tp2.Azimute * c;

            // Check angle diference
            var dAzim = azim2 - azim1;

            if (Math.Abs(dAzim) > Math.PI)
            {
                if (azim2 > azim1)
                {
                    if (azim2 > 0)
                    {
                        azim2 -= 2 * Math.PI;
                    }
                    else
                    {
                        azim1 += 2 * Math.PI;
                    }
                }
                else
                {
                    if (azim1 > 0)
                    {
                        azim1 -= 2 * Math.PI;
                    }
                    else
                    {
                        azim2 += 2 * Math.PI;
                    }
                }
            }

            // Radius-of-Curvature in the vertical plane
            var rv = (l2 - l1) / (incli2 - incl1);

            // Prevents overflow
            if (rv.Equals(0.0))
            {
                return false;
            }

            var f = (zx - z1) / rv + Math.Sin(incl1);

            // Prevents overflow
            var f2Decimais = (double)Math.Truncate((decimal)f * 100) / 100;
            if (Math.Abs(f2Decimais) > 1.0)
            {
                return false;
            }

            // Obtain Inclinação of the required point
            var inclx = (zx - z1) * (incli2 - incl1) / (z2 - z1) + incl1;

            // Length increment
            var dL = rv * (inclx - incl1);

            // Rh = Radius in the horizontal plane (constant in the segment)
            var rh = rv * (Math.Cos(incl1) - Math.Cos(incli2)) / (azim2 - azim1);

            // Prevents overflow
            if (rh.Equals(0.0))
            {
                return false;
            }

            // Obtain Azimute of the required point
            var azimx = azim1 + rv / rh * (Math.Cos(incl1) - Math.Cos(inclx));

            // North/South course coordinate
            var dNs = rh * (Math.Sin(azimx) - Math.Sin(azim1));

            // East/West course coordinate
            var dEw = rh * (Math.Cos(azim1) - Math.Cos(azimx));

            // Dog leg severity
            dls = 0.0;

            // Return parameters
            pm = tp1.Pm.Valor + dL;
            inclinação = inclx / c;
            azimute = azimx / c;
            ns = tp1.NS + dNs;
            ew = tp1.EW + dEw;
            polCoordDispl = Math.Sqrt(Math.Pow(ns, 2) + Math.Pow(ew, 2));
            polCoordDirec = Math.Atan2(ew, ns) / c;

            return true;
        }

        // incl1 != incl2, azim1  = azim2
        private static void Case2Compute(PontoDeTrajetória tp1, ref PontoDeTrajetória tp2)
        {
            const double c = Math.PI / 180.0;

            // Initial point measured depth (well path length)
            var l1 = tp1.Pm.Valor;

            // Final point measured depth (well path length)
            var l2 = tp2.Pm.Valor;

            // Initial point Inclinação
            var incl1 = tp1.Inclinação * c;

            // Final point Inclinação
            var incl2 = tp2.Inclinação * c;

            // Initial point Azimute
            var azim1 = tp1.Azimute * c;

            // Final point Azimute
            var azim2 = tp2.Azimute * c;

            // Check angle diference
            var dAzim = azim2 - azim1;

            if (Math.Abs(dAzim) > Math.PI)
            {
                if (azim2 > azim1)
                {
                    if (azim2 > 0.0)
                    {
                        azim2 -= 2.0 * Math.PI;
                    }
                    else
                    {
                        azim1 += 2.0 * Math.PI;
                    }
                }
                else
                {
                    if (azim1 > 0.0)
                    {
                        azim1 -= 2.0 * Math.PI;
                    }
                    else
                    {
                        azim2 += 2.0 * Math.PI;
                    }
                }
            }

            // Radius-of-Curvature in the vertical plane
            var rv = (l2 - l1) / (incl2 - incl1);

            // Vertical increment
            var dZ = rv * (Math.Sin(incl2) - Math.Sin(incl1));

            // Horizontal increment
            var dH = rv * (Math.Cos(incl1) - Math.Cos(incl2));

            // North/South course coordinate
            var dNs = dH * Math.Cos(azim2);

            // East/West course coordinate
            var dEw = dH * Math.Sin(azim2);

            // Dog leg severity
            const double dls = 0;

            // Update parameters
            tp2.Pv = new Profundidade(tp1.Pv.Valor + dZ);
            tp2.EW = tp1.EW + dEw;
            tp2.NS = tp1.NS + dNs;
            tp2.DLS = dls;
            tp2.PolCoordDispl = Math.Sqrt(Math.Pow(tp2.NS, 2) + Math.Pow(tp2.EW, 2));
            tp2.PolCoordDirec = Math.Atan2(tp2.EW, tp2.NS) / c;
        }

        private static void Case2PmToParameters(double pm, PontoDeTrajetória tp1, PontoDeTrajetória tp2, out double pv, out double inclinação, out double azimute, out double ns, out double ew, out double dls, out double polCoordDispl, out double polCoordDirec)
        {
            // Obtain measured depth
            var lx = pm;
            var l1 = tp1.Pm.Valor;
            var l2 = tp2.Pm.Valor;

            const double c = Math.PI / 180.0;

            var incl1 = tp1.Inclinação * c;
            var incl2 = tp2.Inclinação * c;

            var azim1 = tp1.Azimute * c;

            // Obtain Inclinação of the required point
            var inclx = incl1 + (lx - l1) * (incl2 - incl1) / (l2 - l1);

            // Rv = Radius in the vertical plane (constant in the segment)
            var rv = (l2 - l1) / (incl2 - incl1);

            // Vertical increment
            var dZ = rv * (Math.Sin(inclx) - Math.Sin(incl1));

            // Obtain Azimute of the required point
            var azimx = azim1;

            // Horizontal increment
            var dH = rv * (Math.Cos(incl1) - Math.Cos(inclx));

            // North/South course coordinate
            var dNs = dH * Math.Cos(azimx);

            // East/West course coordinate
            var dEw = dH * Math.Sin(azimx);

            // Dog leg severity
            dls = 0;

            // Return parameters
            pv = tp1.Pv.Valor + dZ;
            inclinação = inclx / c;
            azimute = azimx / c;
            ns = tp1.NS + dNs;
            ew = tp1.EW + dEw;
            polCoordDispl = Math.Sqrt(Math.Pow(ns, 2) + Math.Pow(ew, 2));
            polCoordDirec = Math.Atan2(ew, ns) / c;
        }

        private static bool Case2PvToParameters(double pv, PontoDeTrajetória tp1, PontoDeTrajetória tp2, out double pm, out double inclinação, out double azimute, out double ns, out double ew, out double dls, out double polCoordDispl, out double polCoordDirec)
        {
            // Obtain vertical depth
            var zx = pv;
            var z1 = tp1.Pv.Valor;
            var z2 = tp2.Pv.Valor;

            var l1 = tp1.Pm.Valor;
            var l2 = tp2.Pm.Valor;

            const double c = Math.PI / 180.0;

            var incl1 = tp1.Inclinação * c;
            var incl2 = tp2.Inclinação * c;

            var azim1 = tp1.Azimute * c;

            // Rv = Radius in the vertical plane (constant in the segment)
            var rv = (l2 - l1) / (incl2 - incl1); // obs: no caso2, incl2!=incl1

            // Prevents overflow
            if (rv.Equals(0.0)) // obs: sempre diferente de zero, uma vez que os pontos da sessão tem pms diferentes.
            {
                pm = 0;
                inclinação = 0;
                azimute = 0;
                ns = 0;
                ew = 0;
                dls = 0;
                polCoordDispl = 0;
                polCoordDirec = 0;
                return false;
                //throw new ArgumentException($"GeradorPontosTrajetória: tentativa de gerar um ponto com pv = {pv} entre dois pontos com mesmo pm = {l1}");
            }

            var f = (zx - z1) / rv + Math.Sin(incl1);

            // Prevents overflow
            var f2Decimais = (double)Math.Truncate((decimal)f * 100) / 100;
            if (Math.Abs(f2Decimais) > 1.0)
            {
                pm = 0;
                inclinação = 0;
                azimute = 0;
                ns = 0;
                ew = 0;
                dls = 0;
                polCoordDispl = 0;
                polCoordDirec = 0;
                return false;
                //throw new ArgumentException($"GeradorPontosTrajetória: tentativa de obter um ângulo com seno > 1");
            }

            // Obtain Inclinação of the required point
            var inclx = Math.Asin(f);

            // Length increment
            var dL = rv * (inclx - incl1);

            // Obtain Azimute of the required point
            var azimx = azim1;

            // Horizontal increment
            var dH = rv * (Math.Cos(incl1) - Math.Cos(inclx));

            // North/South course coordinate
            var dNs = dH * Math.Cos(azimx);

            // East/West course coordinate
            var dEw = dH * Math.Sin(azimx);

            // Dog leg severity
            dls = 0.0;

            // Return parameters
            pm = tp1.Pm.Valor + dL;
            inclinação = inclx / c;
            azimute = azimx / c;
            ns = tp1.NS + dNs;
            ew = tp1.EW + dEw;
            polCoordDispl = Math.Sqrt(Math.Pow(ns, 2) + Math.Pow(ew, 2));
            polCoordDirec = Math.Atan2(ew, ns) / c;

            return true;
        }

        // incl1 = incl2, azim1  != azim2
        private static void Case3Compute(PontoDeTrajetória tp1, ref PontoDeTrajetória tp2)
        {
            const double c = Math.PI / 180.0;

            // Initial point measured depth (well path length)
            var l1 = tp1.Pm.Valor;

            // Final point measured depth (well path length)
            var l2 = tp2.Pm.Valor;

            // Initial point Inclinação
            var incl1 = tp1.Inclinação * c;

            // Final point Inclinação
            var incl2 = tp2.Inclinação * c;

            // Initial point Azimute
            var azim1 = tp1.Azimute * c;

            // Final point Azimute
            var azim2 = tp2.Azimute * c;

            // Check angle diference
            var dAzim = azim2 - azim1;

            if (Math.Abs(dAzim) > Math.PI)
            {
                if (azim2 > azim1)
                {
                    if (azim2 > 0)
                    {
                        azim2 -= 2 * Math.PI;
                    }
                    else
                    {
                        azim1 += 2 * Math.PI;
                    }
                }
                else
                {
                    if (azim1 > 0)
                    {
                        azim1 -= 2 * Math.PI;
                    }
                    else
                    {
                        azim2 += 2 * Math.PI;
                    }
                }
            }

            // Length increment
            var dL = l2 - l1;

            // Vertical increment
            var dZ = dL * Math.Cos(incl2);

            // Horizontal increment
            var dH = dL * Math.Sin(incl2);

            // Radius-of-Curvature in the horizontal plane
            var rh = dH / (azim2 - azim1);

            // North/South course coordinate
            var dNs = rh * (Math.Sin(azim2) - Math.Sin(azim1));

            // East/West course coordinate
            var dEw = rh * (Math.Cos(azim1) - Math.Cos(azim2));

            // Dog leg severity
            const double dls = 0;

            // Update parameters
            tp2.Pv = new Profundidade(tp1.Pv.Valor + dZ);
            tp2.EW = tp1.EW + dEw;
            tp2.NS = tp1.NS + dNs;
            tp2.DLS = dls;
            tp2.PolCoordDispl = Math.Sqrt(Math.Pow(tp2.NS, 2) + Math.Pow(tp2.EW, 2));
            tp2.PolCoordDirec = Math.Atan2(tp2.EW, tp2.NS) / c;
        }

        private static void Case3PmToParameters(double pm, PontoDeTrajetória tp1, PontoDeTrajetória tp2, out double pv, out double inclinação, out double azimute, out double ns, out double ew, out double dls, out double polCoordDispl, out double polCoordDirec)
        {
            // Obtain measured depth
            var lx = pm;
            var l1 = tp1.Pm.Valor;
            var l2 = tp2.Pm.Valor;

            const double c = Math.PI / 180.0;

            var incl1 = tp1.Inclinação * c;

            var azim1 = tp1.Azimute * c;
            var azim2 = tp2.Azimute * c;

            // Check angle diference
            var dAzim = azim2 - azim1;

            if (Math.Abs(dAzim) > Math.PI)
            {
                if (azim2 > azim1)
                {
                    if (azim2 > 0)
                    {
                        azim2 -= 2 * Math.PI;
                    }
                    else
                    {
                        azim1 += 2.0 * Math.PI;
                    }
                }
                else
                {
                    if (azim1 > 0)
                    {
                        azim1 -= 2 * Math.PI;
                    }
                    else
                    {
                        azim2 += 2 * Math.PI;
                    }
                }
            }

            // Obtain Inclinação of the required point
            var inclx = incl1;

            // Length increment
            var dL = lx - l1;

            // Vertical increment
            var dZ = dL * Math.Cos(inclx);

            // Horizontal increment
            var dH = dL * Math.Sin(inclx);

            // Rh = Radius in the horizontal plane (constant in the segment)
            var rh = Math.Abs(inclx) > 0.0001 ? (l2 - l1) * Math.Sin(inclx) / (azim2 - azim1) : 0.0;

            // Obtain Azimute of the required point
            var azimx = azim1 + dL * (azim2 - azim1) / (l2 - l1);

            // North/South course coordinate
            var dNs = rh * (Math.Sin(azimx) - Math.Sin(azim1));

            // East/West course coordinate
            var dEw = rh * (Math.Cos(azim1) - Math.Cos(azimx));

            // Return parameters
            pv = tp1.Pv.Valor + dZ;
            inclinação = inclx / c;
            azimute = azimx / c;
            ns = tp1.NS + dNs;
            ew = tp1.EW + dEw;
            dls = 0.0;
            polCoordDispl = Math.Sqrt(Math.Pow(ns, 2) + Math.Pow(ew, 2));
            polCoordDirec = Math.Atan2(ew, ns) / c;
        }

        private static bool Case3PvToParameters(double pv, PontoDeTrajetória tp1, PontoDeTrajetória tp2, out double pm, out double inclinação, out double azimute, out double ns, out double ew, out double dls, out double polCoordDispl, out double polCoordDirec)
        {
            // Obtain vertical depth
            var zx = pv;
            var z1 = tp1.Pv.Valor;
            var z2 = tp2.Pv.Valor;

            var l1 = tp1.Pm.Valor;
            var l2 = tp2.Pm.Valor;

            const double c = Math.PI / 180.0;

            var incl1 = tp1.Inclinação * c;

            var azim1 = tp1.Azimute * c;
            var azim2 = tp2.Azimute * c;

            // Check angle diference
            var dazim = azim2 - azim1;

            if (Math.Abs(dazim) > Math.PI)
            {
                if (azim2 > azim1)
                {
                    if (azim2 > 0)
                    {
                        azim2 -= 2 * Math.PI;
                    }
                    else
                    {
                        azim1 += 2 * Math.PI;
                    }
                }
                else
                {
                    if (azim1 > 0)
                    {
                        azim1 -= 2 * Math.PI;
                    }
                    else
                    {
                        azim2 += 2 * Math.PI;
                    }
                }
            }

            // Obtain Inclinação of the required point
            var inclx = incl1;

            var f = Math.Cos(inclx);

            // Length increment
            var dL = f.Equals(0.0) ? 0 : (zx - z1) / f;

            // Rh = Radius in the horizontal plane (constant in the segment)
            var rh = Math.Abs(inclx) > 0.0001 ? (l2 - l1) * Math.Sin(inclx) / (azim2 - azim1) : 0.0;

            // Obtain Azimute of the required point
            var azimx = azim1 + dL * (azim2 - azim1) / (l2 - l1);

            // North/South course coordinate
            var dns = rh * (Math.Sin(azimx) - Math.Sin(azim1));

            // East/West course coordinate
            var dew = rh * (Math.Cos(azim1) - Math.Cos(azimx));

            // Return parameters
            pm = tp1.Pm.Valor + dL;
            inclinação = inclx / c;
            azimute = azimx / c;
            ns = tp1.NS + dns;
            ew = tp1.EW + dew;
            dls = 0.0;
            polCoordDispl = Math.Sqrt(Math.Pow(ns, 2) + Math.Pow(ew, 2));
            polCoordDirec = Math.Atan2(ew, ns) / c;

            return true;
        }

        // incl1 = incl2, azim1  = azim2
        private static void Case4Compute(PontoDeTrajetória tp1, ref PontoDeTrajetória tp2)
        {
            const double c = Math.PI / 180.0;

            // Initial point measured depth (well path length)
            var l1 = tp1.Pm.Valor;

            // Final point measured depth (well path length)
            var l2 = tp2.Pm.Valor;

            // Initial point Inclinação
            var incl1 = tp1.Inclinação * c;

            // Final point Inclinação
            var incl2 = tp2.Inclinação * c;

            // Initial point Azimute
            var azim1 = tp1.Azimute * c;

            // Final point Azimute
            var azim2 = tp2.Azimute * c;

            // Check angle diference
            var dAzim = azim2 - azim1;

            if (Math.Abs(dAzim) > Math.PI)
            {
                if (azim2 > azim1)
                {
                    if (azim2 > 0)
                    {
                        azim2 -= 2 * Math.PI;
                    }
                    else
                    {
                        azim1 += 2 * Math.PI;
                    }
                }
                else
                {
                    if (azim1 > 0)
                    {
                        azim1 -= 2 * Math.PI;
                    }
                    else
                    {
                        azim2 += 2 * Math.PI;
                    }
                }
            }

            // Length increment
            var dL = l2 - l1;

            // Vertical increment
            var dZ = dL * Math.Cos(incl2);

            // Horizontal increment
            var dH = dL * Math.Sin(incl2);

            // North/South course coordinate
            var dNs = dH * Math.Cos(azim2);

            // East/West course coordinate
            var dEw = dH * Math.Sin(azim2);

            // Dog leg severity
            var dls = 0.0;

            // Update parameters
            tp2.Pv = new Profundidade(tp1.Pv.Valor + dZ);
            tp2.EW = tp1.EW + dEw;
            tp2.NS = tp1.NS + dNs;
            tp2.DLS = dls;
            tp2.PolCoordDispl = Math.Sqrt(Math.Pow(tp2.NS, 2) + Math.Pow(tp2.EW, 2));
            tp2.PolCoordDirec = Math.Atan2(tp2.EW, tp2.NS) / c;
        }

        private static bool Case4PmToParameters(double pm, PontoDeTrajetória tp1, PontoDeTrajetória tp2, out double pv, out double inclinação, out double azimute, out double ns, out double ew, out double dls, out double polCoordDispl, out double polCoordDirec)
        {
            // Obtain measured depth
            var lx = pm;
            var l1 = tp1.Pm.Valor;

            const double c = Math.PI / 180.0;

            var incl1 = tp1.Inclinação * c;
            var azim1 = tp2.Azimute * c;

            // Obtain Inclinação of the required point
            var inclx = incl1;

            // Length increment
            var dL = lx - l1;

            // Vertical increment
            var dz = dL * Math.Cos(inclx);

            // Horizontal increment
            var dH = dL * Math.Sin(inclx);

            // Obtain Azimute of the required point
            var azimx = azim1;

            // North/South course coordinate
            var dns = dH * Math.Cos(azimx);

            // East/West course coordinate
            var dew = dH * Math.Sin(azimx);

            // Dog leg severity
            dls = 0.0;

            // Return parameters
            pv = tp1.Pv.Valor + dz;
            inclinação = inclx / c;
            azimute = azimx / c;
            ns = tp1.NS + dns;
            ew = tp1.EW + dew;
            polCoordDispl = Math.Sqrt(Math.Pow(ns, 2) + Math.Pow(ew, 2));
            polCoordDirec = Math.Atan2(ew, ns) / c;

            return true;
        }

        private static bool Case4PvToParameters(double pv, PontoDeTrajetória tp1, PontoDeTrajetória tp2, out double pm, out double inclinação, out double azimute, out double ns, out double ew, out double dls, out double polCoordDispl, out double polCoordDirec)
        {
            // Obtain vertical depth
            var zx = pv;
            var z1 = tp1.Pv.Valor;
            var z2 = tp2.Pv.Valor;

            var l1 = tp1.Pm.Valor;
            var l2 = tp2.Pm.Valor;

            const double c = Math.PI / 180.0;

            var incl1 = tp1.Inclinação * c;
            var azim1 = tp1.Azimute * c;

            // Obtain Inclinação of the required point
            var inclx = incl1;

            var f = Math.Cos(inclx);

            // Prevents overflow
            if (f.Equals(0.0))
            {
                pm = 0;
                inclinação = 0;
                azimute = 0;
                ns = 0;
                ew = 0;
                dls = 0;
                polCoordDispl = 0;
                polCoordDirec = 0;
                return false;
            }

            // Length increment
            var dL = (zx - z1) / f;

            // Obtain Azimute of the required point
            var azimx = azim1;

            // Horizontal increment
            var dH = dL * Math.Sin(inclx);

            // North/South course coordinate
            var dns = dH * Math.Cos(azimx);

            // East/West course coordinate
            var dew = dH * Math.Sin(azimx);

            // Dog leg severity
            dls = 0.0;

            // Return parameters
            pm = tp1.Pm.Valor + dL;
            inclinação = inclx / c;
            azimute = azimx / c;
            ns = tp1.NS + dns;
            ew = tp1.EW + dew;
            polCoordDispl = Math.Sqrt(Math.Pow(ns, 2) + Math.Pow(ew, 2));
            polCoordDirec = Math.Atan2(ew, ns) / c;

            return true;
        }

        //Case 5  - Minimum Curvature
        private static void Case5Compute(PontoDeTrajetória tp1, ref PontoDeTrajetória tp2)
        {
            var c = Math.PI / 180.0;

            var dL = tp2.Pm.Valor - tp1.Pm.Valor;

            // Initial point Inclinação
            var incl1 = tp1.Inclinação * c;

            // Final point Inclinação
            var incl2 = tp2.Inclinação * c;

            // Initial point Azimute
            var azim1 = tp1.Azimute * c;

            // Final point Azimute
            var azim2 = tp2.Azimute * c;

            // Compute Dog-Leg angle
            var a = Math.Cos(incl1) * Math.Cos(incl2) + Math.Sin(incl1) * Math.Sin(incl2) * Math.Cos(azim2 - azim1);
            if (a > 1.0)
            {
                a = 1.0;
            }
            if (a < -1.0)
            {
                a = -1.0;
            }
            var dla = Math.Acos(a);

            var f = dla < 0.01 ? 1.0 : 2.0 / dla * Math.Tan(dla / 2.0);
            var dNs = dL * (Math.Sin(incl1) * Math.Cos(azim1) + Math.Sin(incl2) * Math.Cos(azim2)) * f / 2.0;
            var dEw = dL * (Math.Sin(incl1) * Math.Sin(azim1) + Math.Sin(incl2) * Math.Sin(azim2)) * f / 2.0;
            var dZ = dL * (Math.Cos(incl1) + Math.Cos(incl2)) * f / 2.0;

            // Compute dog leg severity
            var dls = dla / (c * dL) * 30.0;

            tp2.Pv = new Profundidade(tp1.Pv.Valor + dZ);
            tp2.EW = tp1.EW + dEw;
            tp2.NS = tp1.NS + dNs;
            tp2.DLS = dls;
            tp2.PolCoordDispl = Math.Sqrt(Math.Pow(tp2.NS, 2) + Math.Pow(tp2.EW, 2));
            tp2.PolCoordDirec = Math.Atan2(tp2.EW, tp2.NS) / c;
        }

        private static bool Case5PMToParameters(double pm, PontoDeTrajetória tp1, PontoDeTrajetória tp2, out double pv, out double inclinação, out double azimute, out double ns, out double ew, out double dls, out double polCoordDispl, out double polCoordDirec)
        {
            double r;
            var vciX = 0d;
            var vciY = 0d;
            var vciZ = 0d;

            // Obtain measured depths
            var lx = pm;
            var l1 = tp1.Pm.Valor;
            var l2 = tp2.Pm.Valor;

            const double c = Math.PI / 180.0;

            var l = l2 - l1;

            var incl1 = tp1.Inclinação * c;
            var incl2 = tp2.Inclinação * c;

            var azim1 = tp1.Azimute * c;
            var azim2 = tp2.Azimute * c;

            // Length increment
            var dL = lx - l1;

            // Initial point tangent vectors
            var viX = Math.Sin(incl1) * Math.Cos(azim1);
            var viY = Math.Sin(incl1) * Math.Sin(azim1);
            var viZ = Math.Cos(incl1);

            // Final point tangent vectors
            var vfX = Math.Sin(incl2) * Math.Cos(azim2);
            var vfY = Math.Sin(incl2) * Math.Sin(azim2);
            var vfZ = Math.Cos(incl2);

            // Compute radius R
            var a = AngleV1V2(viX, viY, viZ, vfX, vfY, vfZ);
            if (Math.Abs(a) < 0.0001)
                r = double.MaxValue;
            else
                r = l / a;

            if (Math.Abs(l) < 0.0001 ||
                (Math.Abs(incl1 - incl2) < 0.0001 && (Math.Abs(incl1) < 0.0001 || Math.Abs(azim1 - azim2) < 0.0001)))
            {
                vciX = 0.0;
                vciY = 0.0;
                vciZ = 0.0;
            }
            else
            {
                var vnX = 0d;
                var vnY = 0d;
                var vnZ = 0d;
                ProdVet(viX, viY, viZ, vfX, vfY, vfZ, ref vnX, ref vnY, ref vnZ);
                ProdVet(viX, viY, viZ, vnX, vnY, vnZ, ref vciX, ref vciY, ref vciZ);
            }

            var ang = Math.Abs(r) < 0.0001 ? 0.0 : dL / r;

            var sn = Math.Sin(ang);
            var cs = Math.Cos(ang);

            var dNs = r * (cs - 1.0) * vciX + r * sn * viX;
            var dEw = r * (cs - 1.0) * vciY + r * sn * viY;
            var dZ = r * (cs - 1.0) * vciZ + r * sn * viZ;

            // Derivative
            var vtX = -sn * vciX + cs * viX;
            var vtY = -sn * vciY + cs * viY;
            var vtZ = -sn * vciZ + cs * viZ;

            if (vtZ < -1.0)
            {
                vtZ = -1.0;
            }
            if (vtZ > 1.0)
            {
                vtZ = 1.0;
            }
            var inclx = Math.Abs(Math.Acos(vtZ));
            var azimx = Math.Atan2(vtY, vtX);

            // Return parameters
            pv = tp1.Pv.Valor + dZ;
            inclinação = inclx / c;
            azimute = azimx / c;
            ns = tp1.NS + dNs;
            ew = tp1.EW + dEw;
            dls = tp2.DLS;
            polCoordDispl = Math.Sqrt(Math.Pow(ns, 2) + Math.Pow(ew, 2));
            polCoordDirec = Math.Atan2(ew, ns) / c;

            return true;
        }

        private static bool Case5PMToParameters(PontoDeTrajetória tpx, PontoDeTrajetória tp1, PontoDeTrajetória tp2, out double pv, out double inclinação, out double azimute, out double ns, out double ew, out double dls, out double polCoordDispl, out double polCoordDirec)
        {
            double r;
            var vciX = 0d;
            var vciY = 0d;
            var vciZ = 0d;

            // Obtain measured depths
            var lx = tpx.Pm.Valor;
            var l1 = tp1.Pm.Valor;
            var l2 = tp2.Pm.Valor;

            const double c = Math.PI / 180.0;

            var l = l2 - l1;

            var incl1 = tp1.Inclinação * c;
            var incl2 = tp2.Inclinação * c;

            var azim1 = tp1.Azimute * c;
            var azim2 = tp2.Azimute * c;

            // Length increment
            var dL = lx - l1;

            // Initial point tangent vectors
            var viX = Math.Sin(incl1) * Math.Cos(azim1);
            var viY = Math.Sin(incl1) * Math.Sin(azim1);
            var viZ = Math.Cos(incl1);

            // Final point tangent vectors
            var vfX = Math.Sin(incl2) * Math.Cos(azim2);
            var vfY = Math.Sin(incl2) * Math.Sin(azim2);
            var vfZ = Math.Cos(incl2);

            // Compute radius R
            var a = AngleV1V2(viX, viY, viZ, vfX, vfY, vfZ);
            if (Math.Abs(a) < 0.0001)
                r = double.MaxValue;
            else
                r = l / a;

            if (Math.Abs(l) < 0.0001 ||
                (Math.Abs(incl1 - incl2) < 0.0001 && (Math.Abs(incl1) < 0.0001 || Math.Abs(azim1 - azim2) < 0.0001)))
            {
                vciX = 0.0;
                vciY = 0.0;
                vciZ = 0.0;
            }
            else
            {
                var vnX = 0d;
                var vnY = 0d;
                var vnZ = 0d;
                ProdVet(viX, viY, viZ, vfX, vfY, vfZ, ref vnX, ref vnY, ref vnZ);
                ProdVet(viX, viY, viZ, vnX, vnY, vnZ, ref vciX, ref vciY, ref vciZ);
            }

            var ang = Math.Abs(r) < 0.0001 ? 0.0 : dL / r;

            var sn = Math.Sin(ang);
            var cs = Math.Cos(ang);

            var dNs = r * (cs - 1.0) * vciX + r * sn * viX;
            var dEw = r * (cs - 1.0) * vciY + r * sn * viY;
            var dZ = r * (cs - 1.0) * vciZ + r * sn * viZ;

            // Derivative
            var vtX = -sn * vciX + cs * viX;
            var vtY = -sn * vciY + cs * viY;
            var vtZ = -sn * vciZ + cs * viZ;

            if (vtZ < -1.0)
            {
                vtZ = -1.0;
            }
            if (vtZ > 1.0)
            {
                vtZ = 1.0;
            }
            var inclx = Math.Abs(Math.Acos(vtZ));
            var azimx = Math.Atan2(vtY, vtX);

            // Return parameters
            pv = tp1.Pv.Valor + dZ;
            inclinação = inclx / c;
            azimute = azimx / c;
            ns = tp1.NS + dNs;
            ew = tp1.EW + dEw;
            dls = tp2.DLS;
            polCoordDispl = Math.Sqrt(Math.Pow(tpx.NS, 2) + Math.Pow(tpx.EW, 2));
            polCoordDirec = Math.Atan2(tpx.EW, tpx.NS) / c;

            return true;
        }

        private static bool Case5PvToParameters(double pv, PontoDeTrajetória tp1, PontoDeTrajetória tp2, out double pm, out double inclinação, out double azimute, out double ns, out double ew, out double dls, out double polCoordDispl, out double polCoordDirec)
        {
            double m, v, incl0, azim0, ns0, ew0, polCoordDispl0, polCoordDirec0, lastPv = -1;

            // Obtain required vertical depth
            var vx = pv;

            // Obtain measured depths
            var m1 = tp1.Pm.Valor;
            var m2 = tp2.Pm.Valor;

            var v1 = tp1.Pv.Valor;
            var v2 = tp2.Pv.Valor;

            var flag = false;

            do
            {
                m = m1 + (m2 - m1) / 2.0;

                Case5PMToParameters(m, tp1, tp2, out double pv0, out incl0, out azim0, out ns0, out ew0, out double dls0, out polCoordDispl0, out polCoordDirec0);

                v = (double)Math.Truncate((decimal)pv0 * 100) / 100;
                //v = (double)Math.Truncate((decimal)pv0 * 10000000) / 10000000;

                if (v.Equals(lastPv) || Math.Abs(v - vx) < 0.009)
                {
                    flag = true;
                }
                else
                {
                    lastPv = v;

                    if (v2 > v1)
                    {
                        if (v > vx)
                        {
                            m2 = m;
                            v2 = v;
                        }
                        else
                        {
                            m1 = m;
                            v1 = v;
                        }
                    }
                    else
                    {
                        if (v < vx)
                        {
                            m2 = m;
                            v2 = v;
                        }
                        else
                        {
                            m1 = m;
                            v1 = v;
                        }
                    }
                }
            } while (!flag);

            // Filter to avoid MD < TVD //Rafael 05/11/2007 following Jorge's instruction
            if (m < vx)
            {
                m = vx;
            }

            // Return parameters
            pm = m;
            inclinação = incl0;
            azimute = azim0;
            ns = ns0;
            ew = ew0;
            dls = tp2.DLS;
            polCoordDispl = polCoordDispl0;
            polCoordDirec = polCoordDirec0;

            return true;
        }

        private static double AngleV1V2(double viX, double viY, double viZ, double vfX, double vfY, double vfZ)
        {
            var v1X = viX;
            var v1Y = viY;
            var v1Z = viZ;
            var v2X = vfX;
            var v2Y = vfY;
            var v2Z = vfZ;

            var m1 = Math.Sqrt(v1X * v1X + v1Y * v1Y + v1Z * v1Z);
            if (Math.Abs(m1) < 0.0001)
            {
                m1 = 1.0;
            }

            var m2 = Math.Sqrt(v2X * v2X + v2Y * v2Y + v2Z * v2Z);
            if (Math.Abs(m2) < 0.0001)
            {
                m2 = 1.0;
            }

            v1X = v1X / m1;
            v1Y = v1Y / m1;
            v1Z = v1Z / m1;
            v2X = v2X / m2;
            v2Y = v2Y / m2;
            v2Z = v2Z / m2;

            var pscal = v1X * v2X + v1Y * v2Y + v1Z * v2Z;

            // controllo necessario, nel caso in cui M1 e M2 siano stati
            // arrotondati per difetto:
            if (pscal < -1.0)
            {
                pscal = -1.0;
            }
            if (pscal > 1.0)
            {
                pscal = 1.0;
            }

            return Math.Acos(pscal);
        }

        private static void ProdVet(double v1X, double v1Y, double v1Z, double v2X, double v2Y, double v2Z, ref double vpX,
            ref double vpY, ref double vpZ)
        {
            vpX = v1Y * v2Z - v1Z * v2Y;
            vpY = v1Z * v2X - v1X * v2Z;
            vpZ = v1X * v2Y - v1Y * v2X;

            var mod = Math.Sqrt(vpX * vpX + vpY * vpY + vpZ * vpZ);
            if (Math.Abs(mod) < 0.0001)
            {
                mod = 1.0;
            }

            vpX /= mod;
            vpY /= mod;
            vpZ /= mod;
        }
    }
}
