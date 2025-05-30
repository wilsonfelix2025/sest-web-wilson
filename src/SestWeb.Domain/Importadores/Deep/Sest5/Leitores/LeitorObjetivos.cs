using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Importadores.Deep.Sest5.Leitores
{
    public static class LeitorObjetivos
    {
        public static List<ObjetivoDTO> ObterObjetivos(XDocument xDoc)
        {
            var objetivosLidos =
                xDoc.Root?.Elements("Objetivo").Elements("Obj")
                    .Select(n =>
                        new
                        {
                            PM = n.Attribute("PM").Value,
                            Codigo = n.Attribute("Codigo").Value
                        })
                    .ToList();

            var objetivos = new List<ObjetivoDTO>();

            if (objetivosLidos != null)
                foreach (var objetivosLido in objetivosLidos)
                {
                    string tipoObjetivo;

                    switch (objetivosLido.Codigo)
                    {
                        case "OP":
                            tipoObjetivo = "Primário";
                            break;

                        case "OS":
                            tipoObjetivo = "Secundário";
                            break;

                        default:
                            throw new InvalidOperationException("Tipo de objetivo não esperado");
                    }

                    var objetivo = new ObjetivoDTO(objetivosLido.PM, tipoObjetivo);
                    objetivos.Add(objetivo);
                }

            return objetivos;
        }
    }
}