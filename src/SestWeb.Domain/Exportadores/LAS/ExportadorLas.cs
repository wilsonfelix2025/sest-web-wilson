using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Exportadores.Base;
using SestWeb.Domain.Importadores.Shallow.Utils;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Domain.Exportadores.LAS
{
    public class ExportadorLas: ExportadorBase
    {
        private readonly string ValorNulo = "-99999.0";

        private readonly List<string> Datasets = new List<string>();

        private readonly Dictionary<string, PerfilBase> PerfilPorNome = new Dictionary<string, PerfilBase>();

        public ExportadorLas(Poço poço, List<PerfilBase> perfis, ConfiguraçõesExportador configurações) :
            base(poço, perfis, configurações, TipoExportação.LAS)
        {

        }

        public override byte[] Exportar()
        {
            string output = GerarHeader();
            output += GerarInformaçõesPoço();
            output += GerarDescriçãoCurvas();
            output += GerarValoresCurvas();
            return new UTF8Encoding(true).GetBytes(output);
        }

        private string GerarHeader()
        {
            string header = GerarSeparadorDeSeção();
            header += "~VERSION INFORMATION\n";
            header += GerarLinhaDescritiva("VERS", "", "2.0", "CWLS Log ASCII Standard - Version 2.0");
            header += GerarLinhaDescritiva("WRAP", "", "NO", "One Line per Depth Step");
            return header;
        }

        private string GerarInformaçõesPoço()
        {
            string infoPoço = GerarSeparadorDeSeção();
            infoPoço += "~WELL INFORMATION\n";
            infoPoço += GerarSeparadorDeSeção();
            infoPoço += "#MNEM.UNIT		 DATA		                            DESCRIPTION OF MNEMONIC\n";
            infoPoço += "#---------		 ----		                            -----------------------\n";
            infoPoço += GerarLinhaDescritiva("STRT", "M", StringUtils.FixarCasasDecimais(Poço.Trajetória.PmInicial.Valor, 4), "Profundidade Inicial");
            infoPoço += GerarLinhaDescritiva("STOP", "M", StringUtils.FixarCasasDecimais(Poço.Trajetória.PmFinal.Valor, 4), "Profundidade Final");
            infoPoço += GerarLinhaDescritiva("STEP", "M", StringUtils.FixarCasasDecimais(Configurações.Intervalo, 4), "Intervalo");
            infoPoço += GerarLinhaDescritiva("NULL", "", ValorNulo, "Valor nulo");
            infoPoço += GerarLinhaDescritiva("COMP", "", "PETROLEO BRASILEIRO S/A", "Companhia");
            infoPoço += GerarLinhaDescritiva("ORIG", "", "SEST WEB", "Origem dos dados");
            infoPoço += GerarLinhaDescritiva("WELL", "", Poço.DadosGerais.Identificação.Nome.ToUpperInvariant(), "Poço");
            infoPoço += GerarLinhaDescritiva("FLD", "", Poço.DadosGerais.Identificação.Campo.ToUpperInvariant(), "Campo");
            infoPoço += GerarLinhaDescritiva("LOC", "", "", "Localização");
            infoPoço += GerarLinhaDescritiva("STAT", "", "", "Estado");
            infoPoço += GerarLinhaDescritiva("SRVC", "", "", "Companhia de serviços");
            infoPoço += GerarLinhaDescritiva("DATE", "", "", "Data dos dados");
            infoPoço += GerarLinhaDescritiva("API", "", "", "Código da API");
            infoPoço += GerarLinhaDescritiva("X", "", "??", "X da superfície");
            infoPoço += GerarLinhaDescritiva("Y", "", "??", "Y da superfície");
            infoPoço += GerarLinhaDescritiva("HZCS", "", "UTM", "Sistema Horizontal de Coordenadas");
            infoPoço += GerarLinhaDescritiva("MC", "", "63W", "Meridiano Central");
            infoPoço += GerarLinhaDescritiva("LATI", "", StringUtils.FixarCasasDecimais(Poço.DadosGerais.Geometria.Coordenadas.UtMx, 6), "Latitude");
            infoPoço += GerarLinhaDescritiva("LONG", "", StringUtils.FixarCasasDecimais(Poço.DadosGerais.Geometria.Coordenadas.UtMx, 6), "Longitude");
            infoPoço += GerarLinhaDescritiva("GDAT", "", "", "Datum Geodésico");
            infoPoço += GerarLinhaDescritiva("EKB", "M", "??", "Elevation Kelly Bushing");
            infoPoço += GerarLinhaDescritiva("EGL", "M", "??", "GROUND ELEVATION");
            return infoPoço;
        }

        private string GerarDescriçãoCurvas()
        {
            string descriçãoCurvas = GerarSeparadorDeSeção();
            descriçãoCurvas += "~CURVE INFORMATION\n";
            descriçãoCurvas += GerarSeparadorDeSeção();
            descriçãoCurvas += GerarLinhaDescritiva("MD", "M", "", "Profundidade medida");
            Datasets.Add("MD");


            if (Configurações.DeveExportarTrajetoria)
            {
                descriçãoCurvas += GerarLinhaDescritiva("INCL", "", "", "Inclinação");
                Datasets.Add("INCL");
                descriçãoCurvas += GerarLinhaDescritiva("AZIM", "", "", "Azimute");
                Datasets.Add("AZIM");
            }

            if (Configurações.DeveExportarLitologia)
            {
                descriçãoCurvas += GerarLinhaDescritiva("LITO", "", "", "Litologia");
                Datasets.Add("LITO");
            }

            if (Configurações.DeveExportarPv)
            {
                descriçãoCurvas += GerarLinhaDescritiva("TVD", "M", "", "Profundidade vertical");
                Datasets.Add("TVD");
            }

            if (Configurações.DeveExportarCota)
            {
                descriçãoCurvas += GerarLinhaDescritiva("COTA", "M", "", "Cota");
                Datasets.Add("COTA");
            }

            foreach (var perfil in Perfis)
            {
                var símbolo = perfil.GrupoDeUnidades.UnidadePadrão.Símbolo != "-" ? perfil.GrupoDeUnidades.UnidadePadrão.Símbolo : "";
                descriçãoCurvas += GerarLinhaDescritiva(perfil.Mnemonico, símbolo, "", perfil.Nome.ToUpperInvariant());
                Datasets.Add(perfil.Nome);
                PerfilPorNome.Add(perfil.Nome, perfil);
            }

            return descriçãoCurvas;
        }

        private string GerarValoresCurvas()
        {
            var pmInicial = Configurações.Topo;
            var pmFinal = Configurações.Base;
            var step = Configurações.Intervalo;
            var ultimoValorDiâmetroBroca = 0.0;
            var primeiraProfundidadeComDireção = ObterProfundidadePrimeiroPontoComInclinação();
             
            var valoresCurvas = GerarSeparadorDeSeção();
            valoresCurvas += "~ASCII INFORMATION\n";
            valoresCurvas += GerarSeparadorDeSeção();

            for (var pm = pmInicial; pm < pmFinal; pm += step)
            {
                string linhaAtual = "";

                foreach (var nomeDataset in Datasets)
                {
                    if (nomeDataset == "MD")
                    {
                        linhaAtual += FormatarValor(pm);
                    }
                    else if (nomeDataset == "INCL")
                    {
                        var result = Poço.Trajetória.TryGetPonto(new Profundidade(pm), out var ponto);
                        var valor = -1.0;

                        if (result)
                        {
                            if (pm >= primeiraProfundidadeComDireção)
                            {
                                valor = ponto.Inclinação;
                            }
                            else
                            {
                                valor = 0;
                            }

                            linhaAtual += FormatarValor(valor); 
                        }
                        else
                        {
                            linhaAtual += $"{ValorNulo}\t";
                        }
                    }
                    else if (nomeDataset == "AZIM")
                    {
                        var result = Poço.Trajetória.TryGetPonto(new Profundidade(pm), out var ponto);
                        var valor = -1.0;

                        if (result)
                        {
                            if (pm >= primeiraProfundidadeComDireção)
                            {
                                valor = ponto.Azimute;
                            }
                            else
                            {
                                valor = 0;
                            }

                            linhaAtual += FormatarValor(valor);
                        }
                        else
                        {
                            linhaAtual += $"{ValorNulo}\t";
                        }
                    }
                    else if (nomeDataset == "LITO")
                    {
                        var result = Poço.ObterLitologiaPadrão().TryGetLitoPontoEmPm(Poço.Trajetória, new Profundidade(pm), out var ponto);
                        linhaAtual += result ? $"{ponto.TipoRocha.Numero}\t" : $"{ValorNulo}\t";
                    }
                    else if (nomeDataset == "TVD")
                    {
                        var result = Poço.Trajetória.TryGetPonto(new Profundidade(pm), out var ponto);
                        linhaAtual += result ? FormatarValor(ponto.Pv.Valor) : $"{ValorNulo}\t";
                    }
                    else if (nomeDataset == "COTA")
                    {
                        var result = Poço.Trajetória.TryGetPonto(new Profundidade(pm), out var ponto);
                        linhaAtual += result ? FormatarValor(-ponto.Pv.Valor + Poço.DadosGerais.Geometria.MesaRotativa) : $"{ValorNulo}\t";
                    } 
                    else if (PerfilPorNome.ContainsKey(nomeDataset))
                    {
                        var perfil = PerfilPorNome[nomeDataset];

                        bool result;
                        Ponto ponto;

                        if (perfil.Mnemonico == TiposPerfil.GeTipoPerfil<DIAM_BROCA>().Mnemônico)
                        {
                            result = perfil.TryGetPontoOrPreviousEmPm(Poço.Trajetória, new Profundidade(pm), out ponto);

                            if (ponto.Valor != ultimoValorDiâmetroBroca)
                            {
                                ultimoValorDiâmetroBroca = ponto.Valor;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            result = perfil.TryGetPontoEmPm(Poço.Trajetória, new Profundidade(pm), out ponto);
                        }

                        linhaAtual += result ? FormatarValor(ponto.Valor) : $"{ValorNulo}\t";
                    }
                }

                valoresCurvas += $"{linhaAtual}\n";
            }

            return valoresCurvas;
        }

        private string GerarLinhaDescritiva(string mnemônico,string unidade, string valor, string comentário)
        {
            string mnemônicoComUnidade = $"{mnemônico}.{unidade}";
            string padding = new string(' ', 15 - mnemônicoComUnidade.Length);

            string linha = $"{mnemônicoComUnidade}{padding}{valor}";

            if (comentário != "")
            {
                linha += $"{new string(' ', 50 - linha.Length)}:{comentário}";
            }
            linha += "\n";

            return linha;
        }

        private string FormatarValor(double value)
        {
            return $"{StringUtils.FixarCasasDecimais(value, 4)}\t";
        }

        private string GerarSeparadorDeSeção()
        {
            return "#---------------------------\n";
        }
    }
}
