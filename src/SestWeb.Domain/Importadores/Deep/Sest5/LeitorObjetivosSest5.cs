using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Importadores.Deep.Sest5
{
    public static class LeitorObjetivosSest5
    {
        public static List<ObjetivoDTO> ObterObjetivos(XDocument xDoc)
        {
            var objetivos = new List<ObjetivoDTO>();

            if (xDoc.Root != null)
            {
                var objetivosLidos = xDoc.Root.Elements("Objetivo").Elements("Obj")
                    .Select(n => new
                    {
                        PM = n.Attribute("PM")?.Value,
                        TipoObjetivo = n.Attribute("Codigo")?.Value
                    }).ToList();
                
                foreach (var objetivo in objetivosLidos)
                {
                    var tipo = "";
                    switch (objetivo.TipoObjetivo)
                    {
                        case "OP":
                            tipo = "Primário";
                            break;
                        case "OS":
                            tipo = "Secundário";
                            break;
                    }

                    var objetivoDto = new ObjetivoDTO(objetivo.PM, tipo);
                    objetivos.Add(objetivoDto);
                }
            }

            return objetivos;
        }
    }
}