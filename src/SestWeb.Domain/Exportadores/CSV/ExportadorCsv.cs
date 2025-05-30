using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.RegistrosEventos;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Exportadores.Base;
using SestWeb.Domain.Importadores.Shallow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SestWeb.Domain.Exportadores.CSV
{
    public class ExportadorCsv : ExportadorBase
    {
        private readonly string ValorNulo = "-99999.0";

        private readonly List<string> Datasets = new List<string>();

        private readonly Dictionary<string, PerfilBase> PerfilPorNome = new Dictionary<string, PerfilBase>();

        private readonly string Separador = ";";

        public ExportadorCsv(Poço poço, List<PerfilBase> perfis, ConfiguraçõesExportador configurações) :
            base(poço, perfis, configurações, TipoExportação.CSV)
        {

        }

        public ExportadorCsv(Poço poço, List<RegistroEvento> registros) :
            base(poço, registros)
        {

        }
        public override byte[] Exportar()
        {
            string output = GerarCabeçalho();
            output += GerarValores();
            return new UTF8Encoding(true).GetBytes(output);
        }

        public override byte[] ExportarRegistros()
        {
            string output = Poço.Nome + "\n";
            output += GerarRegistrosPressãoDePoros();
            output += GerarRegistrosEnsaios();
            output += GerarRegistrosTestesAbsorção();
            output += GerarRegistrosOutros();
            output += GerarRegistrosEventos();

            return new UTF8Encoding(true).GetBytes(output);
        }

        private string GerarRegistrosEventos()
        {
            string linha = "";
           
            var registros = Registros.FindAll(r => r.Tipo == TipoRegistroEvento.Evento);

            if (registros.Count > 0)
            {
                linha = "Eventos de Perfuração \n";
                linha = AdicionarColuna(linha, "Eventos de perfuração");
                linha = AdicionarColuna(linha, "PM topo(m)");
                linha = AdicionarColuna(linha, "PV base(m)");
                linha = AdicionarColuna(linha, "Comentário");
                linha += "\n";

                foreach (var reg in registros)
                {
                    bool primeiraLinha = true;
                    linha = AdicionarValor(linha, reg.Nome);
                    foreach (var ponto in reg.Trechos)
                    {
                        if (primeiraLinha == false)
                            linha = AdicionarColuna(linha, " ");

                        if (ponto.Topo == null)
                            linha = AdicionarValor(linha, ponto.Ponto.Pm.Valor.ToString());
                        else
                            linha = AdicionarValor(linha, ponto.Topo.Pm.Valor.ToString());

                        if (ponto.Base == null)
                            linha = AdicionarValor(linha, ponto.Ponto.Pm.Valor.ToString());
                        else
                            linha = AdicionarValor(linha, ponto.Base.Pm.Valor.ToString());

                        linha = AdicionarValor(linha, ponto.Comentário);
                        linha += "\n";
                        primeiraLinha = false;
                    }
                }
            }

            return linha;
        }

        private string GerarRegistrosOutros()
        {
            string linha = "";
            var registrosOutros = new List<string>
            {
                   "Pump in/ flowback", "Perfilagem", "Cascalho angular", "Cascalho lascado", "Cascalho tabular", "Cascalho arredondado",
                "Cascalho outros", "Testemunhagem", "Falha operacional", "Pescaria"
            };

            var registros = Registros.FindAll(r => registrosOutros.Contains(r.Nome));

            if (registros.Count > 0)
            {
                linha = "Registros outros \n";
                linha = AdicionarColuna(linha, "Tipo");
                linha = AdicionarColuna(linha, "PM(m)");
                linha = AdicionarColuna(linha, "PV(m)");
                linha = AdicionarColuna(linha, "Comentário");
                linha += "\n";

                foreach (var reg in registros)
                {
                    linha = AdicionarValor(linha, reg.Nome);
                    foreach (var ponto in reg.Trechos)
                    {
                        linha = AdicionarValor(linha, ponto.Ponto.Pm.Valor.ToString());
                        linha = AdicionarValor(linha, ponto.Ponto.Pv.Valor.ToString());
                        linha = AdicionarValor(linha, ponto.Comentário);
                        linha += "\n";
                    }
                }
            }

            return linha;
        }

        private string GerarRegistrosTestesAbsorção()
        {
            string linha = "";
            var registrosTestes = new List<string>
            {
                "FIT", "XLOT","LOT", "Minifrac", "Microfrac", "Micro TI", "Step rate test"
            };

            var registros = Registros.FindAll(r => registrosTestes.Contains(r.Nome));

            if (registros.Count > 0)
            {
                linha = "Registros de testes de absorção \n";
                linha = AdicionarColuna(linha, "Tipo");
                linha = AdicionarColuna(linha, "PM(m)");
                linha = AdicionarColuna(linha, "PV(m)");
                linha = AdicionarColuna(linha, "Valor(" + registros.First().Unidade + ")");
                linha += "\n";

                foreach (var reg in registros)
                {
                    linha = AdicionarValor(linha, reg.Nome);
                    foreach (var ponto in reg.Trechos)
                    {
                        linha = AdicionarValor(linha, ponto.Ponto.Pm.Valor.ToString());
                        linha = AdicionarValor(linha, ponto.Ponto.Pv.Valor.ToString());
                        linha = AdicionarValor(linha, ponto.Valor.ToString());
                        linha += "\n";
                    }
                }
            }

            return linha;
        }

        private string GerarRegistrosEnsaios()
        {
            string linha = "";
            var registrosEnsaios = new List<string>
            {
                "Ângulo de Atrito", "Permeabilidade","Módulo de Compressibilidade dos Grãos", "Resistência à Compressão Uniaxial", "Resistência à Tração", "Coesão", "Módulo de Young"
            };

            var registros = Registros.FindAll(r => registrosEnsaios.Contains(r.Nome));

            if (registros.Count > 0)
            {
                linha = "Registros de ensaios \n";
                linha = AdicionarColuna(linha, "Propriedade");
                linha = AdicionarColuna(linha, "PM(m)");
                linha = AdicionarColuna(linha, "PV(m)");
                linha = AdicionarColuna(linha, "Valor");
                linha = AdicionarColuna(linha, "Unidade");
                linha += "\n";

                foreach (var reg in registros)
                {
                    linha = AdicionarValor(linha, reg.Nome);
                    foreach (var ponto in reg.Trechos)
                    {
                        linha = AdicionarValor(linha, ponto.Ponto.Pm.Valor.ToString());
                        linha = AdicionarValor(linha, ponto.Ponto.Pv.Valor.ToString());
                        linha = AdicionarValor(linha, ponto.Valor.ToString());
                        linha = AdicionarValor(linha, ponto.Comentário);
                        linha += "\n";
                    }
                }
            }

            return linha;
        }

        private string GerarRegistrosPressãoDePoros()
        {
            string linha = "";
            var registroPressaoPoro = Registros.Find(r => r.Nome == "Pressão de Poros");

            if (registroPressaoPoro != null)
            {
                linha = "Registros de pressão de poros \n";
                linha = AdicionarColuna(linha, "PM(m)");
                linha = AdicionarColuna(linha, "PV(m)");
                linha = AdicionarColuna(linha, "Valor(" + registroPressaoPoro.Unidade + ")");
                linha += "\n";

                foreach (var ponto in registroPressaoPoro.Trechos)
                {
                    linha = AdicionarValor(linha, ponto.Ponto.Pm.Valor.ToString());
                    linha = AdicionarValor(linha, ponto.Ponto.Pv.Valor.ToString());
                    linha = AdicionarValor(linha, ponto.Valor.ToString());
                    linha += "\n";
                }
            }

            return linha;
        }

        private string GerarCabeçalho()
        {
            string cabeçalho = "";
            cabeçalho = AdicionarColuna(cabeçalho, "PM");

            if (Configurações.DeveExportarTrajetoria)
            {
                cabeçalho = AdicionarColuna(cabeçalho, "Inclinação");
                cabeçalho = AdicionarColuna(cabeçalho, "Azimute");
            }

            if (Configurações.DeveExportarLitologia)
            {
                cabeçalho = AdicionarColuna(cabeçalho, "Litologia");
            }

            if (Configurações.DeveExportarPv)
            {
                cabeçalho = AdicionarColuna(cabeçalho, "PV");
            }

            if (Configurações.DeveExportarCota)
            {
                cabeçalho = AdicionarColuna(cabeçalho, "Cota");
            }

            foreach (var perfil in Perfis)
            {
                var símbolo = perfil.GrupoDeUnidades.UnidadePadrão.Símbolo != "-" ? $" ({perfil.GrupoDeUnidades.UnidadePadrão.Símbolo})" : "";
                cabeçalho = AdicionarColuna(cabeçalho, $"{perfil.Nome}{símbolo}");
                PerfilPorNome.Add($"{perfil.Nome}{símbolo}", perfil);
            }

            cabeçalho += "\n";
            return cabeçalho;
        }

        private string GerarValores()
        {
            var pmInicial = Configurações.Topo;
            var pmFinal = Configurações.Base;
            var step = Configurações.Intervalo;
            var linhasArquivo = "";
            var primeiraProfundidadeComDireção = ObterProfundidadePrimeiroPontoComInclinação();

            for (var pm = pmInicial; pm < pmFinal; pm += step)
            {
                string linhaAtual = "";

                foreach (var nomeDataset in Datasets)
                {
                    if (nomeDataset == "PM")
                    {
                        linhaAtual = AdicionarValor(linhaAtual, StringUtils.FixarCasasDecimais(pm, 4));
                    }
                    else if (nomeDataset == "Inclinação")
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

                            linhaAtual = AdicionarValor(linhaAtual, StringUtils.FixarCasasDecimais(valor, 4));
                        }
                        else
                        {
                            linhaAtual = AdicionarValor(linhaAtual, ValorNulo);
                        }
                    }
                    else if (nomeDataset == "Azimute")
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

                            linhaAtual = AdicionarValor(linhaAtual, StringUtils.FixarCasasDecimais(valor, 4));
                        }
                        else
                        {
                            linhaAtual = AdicionarValor(linhaAtual, ValorNulo);
                        }
                    }
                    else if (nomeDataset == "Litologia")
                    {
                        var result = Poço.ObterLitologiaPadrão().TryGetLitoPontoEmPm(Poço.Trajetória, new Profundidade(pm), out var ponto);
                        var valorParaAdicionar = result ? $"{ponto.TipoRocha.Numero}" : ValorNulo;
                        linhaAtual = AdicionarValor(linhaAtual, valorParaAdicionar);
                    }
                    else if (nomeDataset == "PV")
                    {
                        var result = Poço.Trajetória.TryGetPonto(new Profundidade(pm), out var ponto);
                        var valorParaAdicionar = result ? StringUtils.FixarCasasDecimais(ponto.Pv.Valor, 4) : ValorNulo;
                        linhaAtual = AdicionarValor(linhaAtual, valorParaAdicionar);
                    }
                    else if (nomeDataset == "Cota")
                    {
                        var result = Poço.Trajetória.TryGetPonto(new Profundidade(pm), out var ponto);
                        var valorParaAdicionar = result ? StringUtils.FixarCasasDecimais(-ponto.Pv.Valor + Poço.DadosGerais.Geometria.MesaRotativa, 4) : ValorNulo;
                        linhaAtual = AdicionarValor(linhaAtual, valorParaAdicionar);
                    }
                    else if (PerfilPorNome.ContainsKey(nomeDataset))
                    {
                        var perfil = PerfilPorNome[nomeDataset];
                        bool result;
                        Ponto ponto;

                        if (perfil.Mnemonico == TiposPerfil.GeTipoPerfil<DIAM_BROCA>().Mnemônico)
                        {
                            result = perfil.TryGetPontoOrPreviousEmPm(Poço.Trajetória, new Profundidade(pm), out ponto);
                        }
                        else
                        {
                            result = perfil.TryGetPontoEmPm(Poço.Trajetória, new Profundidade(pm), out ponto);
                        }

                        var valorParaAdicionar = result ? StringUtils.FixarCasasDecimais(ponto.Valor, 4) : ValorNulo;
                        linhaAtual = AdicionarValor(linhaAtual, valorParaAdicionar);
                    }
                }

                linhasArquivo += $"{linhaAtual}\n";
            }

            return linhasArquivo;
        }


        private string AdicionarColuna(string cabeçalho, string coluna)
        {
            cabeçalho = AdicionarValor(cabeçalho, coluna);
            Datasets.Add(coluna);
            return cabeçalho;
        }

        private string AdicionarValor(string linha, string valor)
        {
            return linha + $"{valor}{Separador}";
        }
    }
}
