using SestWeb.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SestWeb.Domain.Importadores.Deep.Sest5
{
    public class LeitorRegistroEventosSest5
    {
        public static List<RegistroDTO> ObterRegistros(XDocument xDoc)
        {
            var registros = new List<RegistroDTO>();

            LerRegistrosPressãoDePoros(registros, xDoc);
            LerRegistrosEnsaios(registros, xDoc);
            LerRegistrosTestesDeAbsorção(registros, xDoc);

            return registros;
        }

        public static List<RegistroDTO> ObterEventos(XDocument xDoc)
        {
            var eventos = new List<RegistroDTO>();

            if (xDoc.Root != null)
            {
                var eventosLidos =
                    xDoc.Root.Elements("EventosDePerfuracao").Elements()
                    .Select(n =>
                            new
                            {
                                Tipo = n.Attribute("Evento")?.Value,
                                Topo = n.Descendants().Elements("Topo").Nodes().OfType<XText>().Select(l => l.Value).ToList(),
                                Base = n.Descendants().Elements("Base").Nodes().OfType<XText>().Select(l => l.Value).ToList(),
                                Label = n.Descendants().Elements("Label").ToList()
                            })
                        .ToList();

                foreach (var eventoLido in eventosLidos)
                {
                    var evento = new RegistroDTO();
                    if (eventoLido.Tipo.ToLower() == "gás")
                        evento.Tipo = "Gás de formação";
                    else if (eventoLido.Tipo.ToLower() == "drag")
                        evento.Tipo = "Arraste";
                    else
                        evento.Tipo = eventoLido.Tipo;

                    for (int i = 0; i < eventoLido.Base.Count-1; i++)
                    {
                        var ponto = new PontoRegistroDTO();
                        ponto.Pm = eventoLido.Base[i];
                        ponto.Comentário = eventoLido.Label[i].ToString().Replace("Label","").Replace("<","").Replace(">","").Replace("/","");
                        evento.Pontos.Add(ponto);
                    }                  

                    eventos.Add(evento);
                }

            }

            return eventos;
        }

        private static void LerRegistrosTestesDeAbsorção(List<RegistroDTO> registros, XDocument xDoc)
        {
            if (xDoc.Root != null)
            {
                var registrosLidos =
                    xDoc.Root.Elements("TesteAbsorcao").Elements("TesteAbs")
                        .Select(n =>
                            new
                            {
                                PM = n.Attribute("PM")?.Value,
                                Valor = n.Attribute("PesoPsi")?.Value,
                                Tipo = n.Attribute("Resultado")?.Value
                            })
                        .ToList();

                foreach (var registroLido in registrosLidos)
                {
                    var registro = new RegistroDTO();
                    if (registroLido.Tipo == "A")
                        registro.Tipo = "LOT";
                    else if (registroLido.Tipo == "S")
                        registro.Tipo = "FIT";
                    else
                        registro.Tipo = "Minifrac";

                    registro.Unidade = "psi";

                    var ponto = new PontoRegistroDTO();
                    ponto.Pm = registroLido.PM;
                    ponto.Valor = registroLido.Valor;

                    registro.Pontos.Add(ponto);

                    registros.Add(registro);
                }

            }
        }

        private static void LerRegistrosEnsaios(List<RegistroDTO> registros, XDocument xDoc)
        {
            if (xDoc.Root != null)
            {
                var registrosLidos =
                    xDoc.Root.Elements("RegistroPropriedadeMecanica").Elements("RegPropMec")
                        .Select(n =>
                            new
                            {
                                PV = n.Attribute("PV")?.Value,
                                Valor = n.Attribute("Valor")?.Value,
                                Codigo = n.Attribute("Codigo")?.Value
                            })
                        .ToList();

                foreach (var registroLido in registrosLidos)
                {
                    var registro = new RegistroDTO();
                    registro.Tipo = registroLido.Codigo;
                    registro.Unidade = "psi";

                    var ponto = new PontoRegistroDTO();
                    ponto.Pv = registroLido.PV;
                    ponto.Valor = registroLido.Valor;

                    registro.Pontos.Add(ponto);

                    registros.Add(registro);
                }

            }
        }

        private static void LerRegistrosPressãoDePoros(List<RegistroDTO> registros, XDocument xDoc)
        {
            if (xDoc.Root != null)
            {
                var registrosLidos =
                    xDoc.Root.Elements("RegistroPressaoPoros").Elements("RegPresPoros")
                        .Select(n =>
                            new
                            {
                                PV = n.Attribute("PV")?.Value,
                                Valor = n.Attribute("PressaoPorosPsi")?.Value
                            })
                        .ToList();

                var registro = new RegistroDTO();
                registro.Tipo = "Pressão de Poros";
                registro.Unidade = "psi";

                foreach (var registroLido in registrosLidos)
                {
                    var ponto = new PontoRegistroDTO();
                    ponto.Pv = registroLido.PV;
                    ponto.Valor = registroLido.Valor;

                    registro.Pontos.Add(ponto);
                }

                registros.Add(registro);
            }
        }
    }
}
