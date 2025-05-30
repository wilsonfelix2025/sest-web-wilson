using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Importadores.Leitores.SIGEO
{
    public class LeitorRegistrosDePressãoSIGEO
    {
        public RegistroDTO RegistroDePerfuração { get; private set; }
        private readonly LeitorHelperSIGEO _leitorGeral;
        private readonly IReadOnlyList<string> _linhas;
        private bool _buscarDadosShallow;


        public LeitorRegistrosDePressãoSIGEO(IReadOnlyList<string> linhas, bool buscarDadosShallow)
        {
            _linhas = linhas;
            RegistroDePerfuração = new RegistroDTO();
            _leitorGeral = new LeitorHelperSIGEO();
            _buscarDadosShallow = buscarDadosShallow;
            GetRegistrosDePressão();
        }

        private void GetRegistrosDePressão()
        {
            var indexs = _leitorGeral.EncontrarIdentificadoresDeSeção("REGISTROS DE PRESSAO", _linhas);
            var pontos = new List<PontoRegistroDTO>();

            foreach (var index in indexs)
            {
                var ponto = CarregarPontoDeRegistroDePerfuração(index, _linhas);

                if (ponto == null)
                    continue;

                if (pontos.Any(p => p.Pm.Equals(ponto.Pm)))
                    continue;

                pontos.Add(ponto);

                //se for para buscar shallow, achando a primeira sapata, pode sair do loop
                if (_buscarDadosShallow)
                    break;
            }

            RegistroDePerfuração.Unidade = "psi";
            RegistroDePerfuração.Tipo = "Pressão de Poros";
            RegistroDePerfuração.Pontos = pontos;
        }

        private PontoRegistroDTO CarregarPontoDeRegistroDePerfuração(int index, IReadOnlyList<string> linhas)
        {
            if (index == -1)
                return null;

            Dictionary<string, string> informaçõesDoPonto = CarregarInformações(linhas, index);

            var pmString = informaçõesDoPonto["pm"];
            var valorString = informaçõesDoPonto["pEstática"];

            if (string.IsNullOrEmpty(pmString) || string.IsNullOrEmpty(valorString))
                return null;

            var pm = informaçõesDoPonto["pm"].Trim();
            var cota = informaçõesDoPonto["cota"].Trim();
            var valorPsi = informaçõesDoPonto["pEstática"].Trim();
            //var convGrad = 0.170433;
            var descrição = informaçõesDoPonto["descrição"];
            var qualidade = informaçõesDoPonto["qualidade"];

            //PontoRegistroDePerfuraçãoDTO ponto = new PontoRegistroDePerfuraçãoDTO(pm, valor, string.Empty);
            if (qualidade.ToUpper() != "ESTABILIZADA")
                return null;

            var ponto = new PontoRegistroDTO {Pm = pm, Valor = valorString};
            return ponto;

        }

        private Dictionary<string, string> CarregarInformações(IReadOnlyList<string> linhas, int index)
        {
            Dictionary<string, string> _dados = new Dictionary<string, string>();

            if (index == -1)
                return _dados;
            index += 2;

            string linha;
            do
            {
                linha = linhas[index];

                var pmPattern = @"\bPROF. MEDIDA\b\s*:\s*(?<Valor>\d*\.?\d*)";
                var regexPm = new Regex(pmPattern);

                if (regexPm.IsMatch(linha))
                {
                    var pm = regexPm.Match(linha).Groups[1].Value.Trim();
                    _dados.Add("pm", pm);
                }

                var cotaPattern = @"\bPROF. MEDIDA\b\s*:\s*\d*\.?\d*\s*\(([^)]+)\)";
                var regexCota = new Regex(cotaPattern);

                if (regexCota.IsMatch(linha))
                {
                    var cota = regexCota.Match(linha).Groups[1].Value.Trim();
                    _dados.Add("cota", cota);
                }

                var qualidadePattern = @"\bQUALIDADE ESTATICA\b\s*:[\s\S]*$";
                var regexQualidade = new Regex(qualidadePattern);

                if (regexQualidade.IsMatch(linha))
                {
                    var qualidade = regexQualidade.Match(linha).Value.Split(":")[1].Trim();
                    _dados.Add("qualidade", qualidade);
                }


                var pePattern = @"\bPRESSAO ESTATICA\b\s*:\s*(?<Valor>\d*\.?\d*)";
                var regexPe = new Regex(pePattern);

                if (regexPe.IsMatch(linha))
                {
                    var pEstática = regexPe.Match(linha).Groups[1].Value.Trim();
                    _dados.Add("pEstática", pEstática);
                }

                var descPattern = @"\bCOMENTARIO\b\s*:";
                var regexDesc = new Regex(descPattern);

                if (regexDesc.IsMatch(linha))
                {
                    linha = linhas[++index];
                    var descrição = linha.Trim();
                    linha = linhas[++index];
                    if ( !string.IsNullOrEmpty(linha) && !linha.Contains("______"))
                        descrição += string.Empty + linha.Trim();

                    _dados.Add("descrição", descrição);
                    break;
                }

                index++;
            } while (!linha.Contains("______") && index < linhas.Count);

            return _dados;
        }

    }
}
