using CoreHtmlToImage;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Poço;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SestWeb.Domain.Relatório
{
    public class Relatório
    {
        private string _relatórioEmHtml = "";
        private Poço _poçoObjeto { get; set; }
        private ConfiguraçãoRelatório _configurações { get; set; }
        private string _estiloCss = @"
            <style>
                body {
                    padding: 20px;
                    font-family: monospace;
                    width: 1920px;
                    min-height: 1080px;
                }
                #well-info {
                    margin-bottom: 20px;
                }

                label {
                    color: #999;
                }

                .well-info-row {
                    margin-bottom: 10px;
                }

                #well-info span {
                    font-weight: bold;
                }

                table label {
                    text-align: center;
                    color: black;
                    font-weight: bold;
                }

                tbody img {
                    width: 240px;
                    height: 640px;
                }

                tbody {
                    color: black;
                    font-size: 11px;
                    font-weight: bold;
                    font-family: sans-serif;
                }

                thead th {
                    height: 30px;
                    line-height: 30px;
                }

                table {
                    margin-right: 20px;
                    display: inline-block;
                }

                table.narrow tbody img {
                    width: 80px;
                }
            </style>
        ";

        public Relatório(Poço poço, ConfiguraçãoRelatório configurações)
        {
            _poçoObjeto = poço;
            _configurações = configurações;
            _relatórioEmHtml = GerarHTML();
        }

        private string GerarHTML()
        {
            var offsetBase = 20;
            var offsetAtual = offsetBase;
            var html = _estiloCss;
            html += "<body>";
            html += ObterInícioCabeçalho();
            html += ObterData();
            
            if (_configurações.MostrarNome)
            {
                html += ObterNomePoço();
            }

            if (_configurações.Trajetória.Curvas.Count > 0)
            {
                html += ObterGeometriaDoPoço();
            }

            if (_configurações.MostrarTipoPoço)
            {
                html += ObterAmbiente();
            }

            if (_configurações.MostrarProfundidadeFinal)
            {
                html += ObterPmFinal();
            }

            if (_configurações.MostrarLDA)
            {
                html += ObterLaminaDagua();
            }

            if (_configurações.MostrarMR)
            {
                html += ObterMesaRotativa();
            }

            html += ObterFimSeção();
            html += ObterInícioGráficos();

            var inicioLinhaTracks = "";

            if (_configurações.Trajetória.Data != null)
            {
                inicioLinhaTracks += GerarGráfico(_configurações.Trajetória.Titulo, _configurações.Trajetória.Data);
                offsetBase += 240 + 20;
            }

            if (_configurações.Litologia.Data != null)
            {
                inicioLinhaTracks += GerarGráfico(_configurações.Litologia.Titulo, _configurações.Litologia.Data, true);
                offsetBase += 80 + 20;
            }

            foreach (var estratigrafia in _configurações.Estratigrafias) {
                inicioLinhaTracks += GerarGráfico(estratigrafia.Titulo, estratigrafia.Data, true);
                offsetBase += 80 + 20;
            }

            html += inicioLinhaTracks;
            offsetAtual = offsetBase;

            foreach (var gráfico in _configurações.Graficos)
            {
                if (gráfico.Curvas.Count > 0)
                {
                    if (offsetAtual + 240 + 20 > 1920)
                    {
                        html += ObterFimSeção();
                        html += ObterInícioGráficos();
                        html += inicioLinhaTracks;
                        offsetAtual = offsetBase;
                    }

                    html += GerarGráfico(gráfico.Titulo, gráfico.Data);
                    offsetAtual += 240 + 20;
                }
            }

            html += ObterFimSeção();
            html += "</body>";

            return html;
        }

        public byte[] ExportarBytes()
        {
            var formattedString = _relatórioEmHtml.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            var converter = new HtmlConverter();
            return converter.FromHtmlString(formattedString);
        }

        public async Task<string> Exportar(string caminho, string formato)
        {
            var caminhoCompleto = Path.Combine(caminho, "teste");
            caminhoCompleto = Path.ChangeExtension(caminhoCompleto, formato);

            var bytes = ExportarBytes();

            using (var stream = File.Create(caminhoCompleto))
            {
                await stream.WriteAsync(bytes);
            }

            return caminhoCompleto.Split("\\wwwroot")[caminhoCompleto.Split("\\wwwroot").Length - 1];
        }

        public string GerarLinhaCabeçalho(string label, string valor)
        {
            return $@"
                <div class=""well-info-row"">
                    <label>{label}:</label>
                    <span>{valor}</label>
                </div>
            ";
        }

        public string GerarGráfico(string título, string imagemBase64, bool estreito = false)
        {
            return $@"
                <table{(estreito ? " class=\"narrow\"" : "")}>
                    <thead>
                        <tr><th>{título}</th>
                    </tr></thead>
                    <tbody>
                        <tr><td>
                            <img src={imagemBase64}>
                        </td></tr>
                    </tbody>
                </table>
             ";
        }
        
        private string ObterInícioCabeçalho()
        {
            return @"<div id=""well-info"">";
        }

        private string ObterInícioGráficos() {
            return @"<div id=""charts-row"">";
        }

        private string ObterFimSeção()
        {
            return "</div>";
        }

        private string ObterData()
        {
            return GerarLinhaCabeçalho("Data", DateTime.UtcNow.ToString("dd/MM/yyyy"));
        }

        private string ObterNomePoço()
        {
            return GerarLinhaCabeçalho("Nome do po&ccedil;o", _poçoObjeto.Nome);
        }

        private string ObterGeometriaDoPoço()
        {
            return GerarLinhaCabeçalho("Geometria do po&ccedil;o", _poçoObjeto.Trajetória.ÉVertical ? "Vertical" : "Direcional");
        }

        private string ObterAmbiente()
        {
            return GerarLinhaCabeçalho("Ambiente", _poçoObjeto.DadosGerais.Geometria.CategoriaPoço == CategoriaPoço.OnShore ? "Terrestre" : "Mar&iacute;timo");
        }

        private string ObterPmFinal()
        {
            return GerarLinhaCabeçalho("PM final", _poçoObjeto.Trajetória.ÚltimoPonto.Pm.Valor.ToString() + "m");
        }

        private string ObterMesaRotativa()
        {
            return GerarLinhaCabeçalho("Altura da mesa rotativa", _poçoObjeto.DadosGerais.Geometria.MesaRotativa.ToString() + "m");
        }

        private string ObterLaminaDagua()
        {
            return GerarLinhaCabeçalho("Espessura da LDA", _poçoObjeto.DadosGerais.Geometria.OffShore.LaminaDagua.ToString() + "m");
        }


    }
}
