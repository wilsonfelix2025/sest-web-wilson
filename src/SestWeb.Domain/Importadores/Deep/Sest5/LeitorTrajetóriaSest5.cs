using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Importadores.Deep.Sest5
{
    public static class LeitorTrajetóriaSest5
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xDoc">Documento xml referente ao arquivo Sest 5</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static TrajetóriaDTO ObterTrajetória(XDocument xDoc)
        {
            var trajetóriaElement = xDoc.Root?.Element("Trajetoria");

            MétodoDeCálculoDaTrajetória método;
            var pontosTrajetória = new List<PontoTrajetóriaDTO>();            

            var tipoPoço = LeitorTipoPoço.ObterTipoPoço(xDoc);
            switch (tipoPoço)
            {
                case TipoPoço.Projeto:
                    método = (MétodoDeCálculoDaTrajetória)int.Parse(trajetóriaElement?.Element("TrajProj").Attribute("Metodo")?.Value ?? throw new InvalidOperationException("Não foi possível encontrar método de cálculo de trajetória."));
                    if (método == MétodoDeCálculoDaTrajetória.RaioCurvatura)
                    {
                        pontosTrajetória = xDoc.Root?.Element("TrajProjRaioCurvatura").Elements("TrajProjRaioCurv").Select(n =>
                            new PontoTrajetóriaDTO
                            {
                                Pm = n.Attribute("PM").Value,
                                Inclinação = n.Attribute("Inclinacao").Value,
                                Azimute = n.Attribute("Azimute").Value
                            }).ToList();
                        break;
                    }

                    if (método == MétodoDeCálculoDaTrajetória.MinimaCurvatura)
                    {
                        pontosTrajetória = xDoc.Root?.Element("TrajProjMinCurvatura").Elements("TrajProjMinCurv").Select(n =>
                            new PontoTrajetóriaDTO
                            {
                                Pm = n.Attribute("PM").Value,
                                Inclinação = n.Attribute("Inclinacao").Value,
                                Azimute = n.Attribute("Azimute").Value
                            }).ToList();
                        break;
                    }
                    pontosTrajetória = xDoc.Root?.Element("TrajProjRaioCurvatura").Elements("TrajProjRaioCurv").Select(n => 
                        new PontoTrajetóriaDTO 
                        {
                            Pm = n.Attribute("PM").Value,
                            Inclinação = n.Attribute("Inclinacao").Value,
                            Azimute = n.Attribute("Azimute").Value
                        }).ToList();
                    break;
                case TipoPoço.Retroanalise:
                    método = (MétodoDeCálculoDaTrajetória)int.Parse(trajetóriaElement?.Element("Metodo")?.Value ?? throw new InvalidOperationException("Não foi possível encontrar método de cálculo de trajetória."));
                    pontosTrajetória = trajetóriaElement.Elements("TrajRetro").Select(n =>
                        new PontoTrajetóriaDTO
                        {
                            Pm = n.Attribute("PM").Value,
                            Inclinação = n.Attribute("Inclinacao").Value,
                            Azimute = n.Attribute("Azimute").Value
                        }).ToList();
                    break;
                default:
                    throw new ArgumentException("Tipo de poço não reconhecido!");
            }

            var trajetória = new TrajetóriaDTO { Pontos = pontosTrajetória };

            return trajetória;
        }
    }
}