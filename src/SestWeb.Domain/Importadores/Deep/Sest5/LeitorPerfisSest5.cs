using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Helpers;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using SestWeb.Domain.Entities.PontosEntity;

namespace SestWeb.Domain.Importadores.Deep.Sest5
{
    public class LeitorPerfisSest5
    {
        private readonly XDocument _xDoc;
        private readonly List<PerfilParaImportarDTO> _perfisSelecionados;

        public LeitorPerfisSest5(XDocument xDoc, List<PerfilParaImportarDTO> perfisSelecionados)
        {
            _xDoc = xDoc;
            _perfisSelecionados = perfisSelecionados;
        }

        public List<PerfilDTO> ObterTodosPerfis()
        {
            var perfis = ObterPerfis(_xDoc);
            perfis.AddRange(ObterPropMec(_xDoc));
            perfis.AddRange(ObterTensões(_xDoc));
            perfis.AddRange(ObterGradientes(_xDoc));
            perfis.AddRange(ObterRegistroDeBroca(_xDoc));

            return perfis;
        }

        private List<PerfilDTO> ObterPerfis(XDocument xDoc)
        {
            var perfis = new List<PerfilDTO>();

            if (xDoc.Root != null)
            {
                var linhas = xDoc.Root.Elements("Perfis").Elements("Perfil");

                foreach(var perfilSelecionado in _perfisSelecionados)
                {
                    if(perfilSelecionado.Nome == "DTC" || perfilSelecionado.Nome == "DTS" || 
                        perfilSelecionado.Nome == "DTMC" || perfilSelecionado.Nome == "DTMS" || 
                        perfilSelecionado.Nome == "DENF" || perfilSelecionado.Nome == "DENG" || 
                        perfilSelecionado.Nome == "VCL" || perfilSelecionado.Nome == "GRAY" || 
                        perfilSelecionado.Nome == "REST" || perfilSelecionado.Nome == "CALIP" || 
                        perfilSelecionado.Nome == "PORO")
                    {
                        var perfilDto = new PerfilDTO
                        {
                            Nome = perfilSelecionado.NovoNome ?? perfilSelecionado.Nome,
                            Mnemonico = perfilSelecionado.Tipo
                        };

                        foreach (var linha in linhas)
                        {
                            var pm = linha.Attribute("PM").Value;
                            var pmEmDouble = pm.ToDouble();

                            if (perfilSelecionado.ValorTopo.HasValue && perfilSelecionado.ValorBase.HasValue)
                            {
                                if (pmEmDouble >= perfilSelecionado.ValorTopo && pmEmDouble < perfilSelecionado.ValorBase)
                                {
                                    var valor = linha.Attribute(perfilSelecionado.Nome).Value;
                                    perfilDto.PontosDTO.Add(new PontoDTO { Pm = pm, Valor = valor, Origem = OrigemPonto.Importado });
                                }
                            }
                            else
                            {
                                var valor = linha.Attribute(perfilSelecionado.Nome).Value;
                                perfilDto.PontosDTO.Add(new PontoDTO { Pm = pm, Valor = valor, Origem = OrigemPonto.Importado });
                            }
                        }
                        perfis.Add(perfilDto);
                    }
                }
            }

            return perfis;
        }

        private List<PerfilDTO> ObterPropMec(XDocument xDoc)
        {
            var perfis = new List<PerfilDTO>();

            if (xDoc.Root != null)
            {
                var linhas = xDoc.Root.Elements("PropriedadeMecanica").Elements("PropMec");

                foreach (var perfilSelecionado in _perfisSelecionados)
                {
                    if (perfilSelecionado.Nome == "YOUNG" || perfilSelecionado.Nome == "POISS" ||
                        perfilSelecionado.Nome == "BIOT" || perfilSelecionado.Nome == "RESTR" ||
                        perfilSelecionado.Nome == "ANGAT" || perfilSelecionado.Nome == "COESA" ||
                        perfilSelecionado.Nome == "PERM" || perfilSelecionado.Nome == "KS" ||
                        perfilSelecionado.Nome == "RESCMP")
                    {
                        var perfilDto = new PerfilDTO
                        {
                            Nome = perfilSelecionado.NovoNome ?? perfilSelecionado.Nome,
                            Mnemonico = perfilSelecionado.Tipo
                        };

                        foreach (var linha in linhas)
                        {
                            var pm = linha.Attribute("PM").Value;
                            var pmEmDouble = pm.ToDouble();

                            if (perfilSelecionado.ValorTopo.HasValue && perfilSelecionado.ValorBase.HasValue)
                            {
                                if (pmEmDouble >= perfilSelecionado.ValorTopo && pmEmDouble < perfilSelecionado.ValorBase)
                                {
                                    var valor = linha.Attribute(perfilSelecionado.Nome).Value;
                                    perfilDto.PontosDTO.Add(new PontoDTO { Pm = pm, Valor = valor, Origem = OrigemPonto.Importado });
                                }
                            }
                            else
                            {
                                var valor = linha.Attribute(perfilSelecionado.Nome).Value;
                                perfilDto.PontosDTO.Add(new PontoDTO { Pm = pm, Valor = valor, Origem = OrigemPonto.Importado });
                            }
                        }
                        perfis.Add(perfilDto);
                    }
                }
            }

            return perfis;
        }

        private List<PerfilDTO> ObterTensões(XDocument xDoc)
        {
            var perfis = new List<PerfilDTO>();

            if (xDoc.Root != null)
            {
                var linhas = xDoc.Root.Elements("TensoesInSitu").Elements("TensInSitu");

                foreach (var perfilSelecionado in _perfisSelecionados)
                {
                    if (perfilSelecionado.Nome == "THORm" || perfilSelecionado.Nome == "THORM" ||
                        perfilSelecionado.Nome == "AZTHM")
                    {
                        var perfilDto = new PerfilDTO
                        {
                            Nome = perfilSelecionado.NovoNome ?? perfilSelecionado.Nome,
                            Mnemonico = perfilSelecionado.Tipo
                        };

                        foreach (var linha in linhas)
                        {
                            var pm = linha.Attribute("PM").Value;
                            var pmEmDouble = pm.ToDouble();

                            if (perfilSelecionado.ValorTopo.HasValue && perfilSelecionado.ValorBase.HasValue)
                            {
                                if (pmEmDouble >= perfilSelecionado.ValorTopo && pmEmDouble < perfilSelecionado.ValorBase)
                                {
                                    var valor = linha.Attribute(perfilSelecionado.Nome).Value;
                                    perfilDto.PontosDTO.Add(new PontoDTO { Pm = pm, Valor = valor, Origem = OrigemPonto.Importado });
                                }
                            }
                            else
                            {
                                var valor = linha.Attribute(perfilSelecionado.Nome).Value;
                                perfilDto.PontosDTO.Add(new PontoDTO { Pm = pm, Valor = valor, Origem = OrigemPonto.Importado });
                            }
                        }
                        perfis.Add(perfilDto);
                    }
                }
            }

            return perfis;
        }

        private List<PerfilDTO> ObterGradientes(XDocument xDoc)
        {
            var perfis = new List<PerfilDTO>();

            if (xDoc.Root != null)
            {
                var linhas = xDoc.Root.Elements("Gradientes").Elements("Grad");

                foreach (var perfilSelecionado in _perfisSelecionados)
                {
                    if (perfilSelecionado.Nome == "GSOBR" || perfilSelecionado.Nome == "GPORO" ||
                        perfilSelecionado.Nome == "GFRAT" || perfilSelecionado.Nome == "GCOLS" ||
                        perfilSelecionado.Nome == "GCOLI" || perfilSelecionado.Nome == "GLAMA" ||
                        perfilSelecionado.Nome == "GECD")
                    {
                        var perfilDto = new PerfilDTO
                        {
                            Nome = perfilSelecionado.NovoNome ?? perfilSelecionado.Nome,
                            Mnemonico = perfilSelecionado.Tipo
                        };

                        foreach (var linha in linhas)
                        {
                            var pm = linha.Attribute("PM").Value;
                            var pmEmDouble = pm.ToDouble();

                            if (perfilSelecionado.ValorTopo.HasValue && perfilSelecionado.ValorBase.HasValue)
                            {
                                if (pmEmDouble >= perfilSelecionado.ValorTopo && pmEmDouble < perfilSelecionado.ValorBase)
                                {
                                    var valor = linha.Attribute(perfilSelecionado.Nome).Value;
                                    perfilDto.PontosDTO.Add(new PontoDTO { Pm = pm, Valor = valor, Origem = OrigemPonto.Importado });
                                }
                            }
                            else
                            {
                                var valor = linha.Attribute(perfilSelecionado.Nome).Value;
                                perfilDto.PontosDTO.Add(new PontoDTO { Pm = pm, Valor = valor, Origem = OrigemPonto.Importado });
                            }
                        }
                        perfis.Add(perfilDto);
                    }
                }
            }

            return perfis;
        }

        private List<PerfilDTO> ObterRegistroDeBroca(XDocument xDoc)
        {
            var perfis = new List<PerfilDTO>();

            if (xDoc.Root != null)
            {
                var linhas = xDoc.Root.Elements("RegistroBroca").Elements("RegBroca");

                foreach (var perfilSelecionado in _perfisSelecionados)
                {
                    if (perfilSelecionado.Nome == "DIAM_BROCA")
                    {
                        var perfilDto = new PerfilDTO
                        {
                            Nome = perfilSelecionado.NovoNome ?? perfilSelecionado.Nome,
                            Mnemonico = perfilSelecionado.Tipo
                        };

                        foreach (var linha in linhas)
                        {
                            var pm = linha.Attribute("PM").Value;
                            var pmEmDouble = pm.ToDouble();

                            if (perfilSelecionado.ValorTopo.HasValue && perfilSelecionado.ValorBase.HasValue)
                            {
                                if (pmEmDouble >= perfilSelecionado.ValorTopo && pmEmDouble < perfilSelecionado.ValorBase)
                                {
                                    var valor = new FractionalNumber(linha.Attribute("Diametro")?.Value).DoubleResult.ToString(CultureInfo.InvariantCulture);
                                    perfilDto.PontosDTO.Add(new PontoDTO { Pm = pm, Valor = valor, Origem = OrigemPonto.Importado });
                                }
                            }
                            else
                            {
                                var valor = new FractionalNumber(linha.Attribute("Diametro")?.Value).DoubleResult.ToString(CultureInfo.InvariantCulture);
                                perfilDto.PontosDTO.Add(new PontoDTO { Pm = pm, Valor = valor, Origem = OrigemPonto.Importado });
                            }
                        }
                        perfis.Add(perfilDto);
                    }
                }
            }

            return perfis;
        }
    }
}
