using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;

namespace SestWeb.Domain.Importadores.Leitores.SIGEO
{
    public class LeitorLitologiaInterpretadaSIGEO
    {
        public LitologiaDTO Litologia { get; private set; }
        private readonly LeitorHelperSIGEO _leitorGeral;
        private readonly IReadOnlyList<string> _linhas;
        private readonly List<LitologiaParaImportarDTO> _litologiasSelecionadas;
        private bool _buscarDadosShallow;

        public LeitorLitologiaInterpretadaSIGEO(IReadOnlyList<string> linhas, List<LitologiaParaImportarDTO> litologias, bool buscarDadosShallow)
        {
            _linhas = linhas;
            Litologia = new LitologiaDTO();
            _leitorGeral = new LeitorHelperSIGEO();
            _litologiasSelecionadas = litologias;
            _buscarDadosShallow = buscarDadosShallow;
            GetLitologia();
        }

        private void GetLitologia()
        {
            if (_buscarDadosShallow == false && (_litologiasSelecionadas == null || _litologiasSelecionadas.Count == 0))
                Litologia = null;
            else
            {
                var valores = new List<StringCollection>();
                CarregarInformações(valores);

                for (int i = 0; i < valores.Count; i++)
                {
                    var lito = valores[i];
                    var topoPm = lito[0].Trim();
                    var basePm = lito[2].Trim();

                    var nome = lito[4];

                    //limito o tamanho da string, para não pegar informações extras da linha e não conseguir identificar
                    //o tipo de rocha
                    nome = nome.Length <= 20 ? nome : nome.Substring(0, 20);

                    //faço um tratamento especial para algumas rochas
                    nome = TratarNomes(nome);

                    var ponto = new PontoLitologiaDTO { Pm = topoPm, TipoRocha = nome };

                    Litologia.Pontos.Add(ponto);

                    if (i == valores.Count - 1)
                    {
                        var últimoPonto = new PontoLitologiaDTO { Pm = basePm, TipoRocha = nome };
                        Litologia.Pontos.Add(últimoPonto);
                    }
                }

                Litologia.Classificação = "Interpretada";
                Litologia.Nome = "Litologia";
            }
        }

        private string TratarNomes(string nome)
        {
            if (nome == "IGNEA NAO IDENTIFICA")
                nome = "Ígneas";
            else if (nome == "ESTROMATOLITO ARBUST" || nome == "ESTROMATOLITO ARBORE")
                nome = "Estromatolito";
            return nome;
        }

        private void CarregarInformações(List<StringCollection> valores)
        {
            var index = _leitorGeral.EncontrarIdentificadorDeSeção("LITOLOGIA INTERPRETADA", _linhas);
            if (index == -1)
                return;

            index += 2;

            string linha;
            do
            {
                linha = _linhas[index];
                if (!linha.Contains("TOPO") && !string.IsNullOrEmpty(linha))
                {
                    linha = System.Text.RegularExpressions.Regex.Replace(linha, @"\s{2,}", "  ").Trim();
                    
                    string[] split = linha.Split(new[] { "/",") ", "  ", "\t" }, StringSplitOptions.None);
                    var collection = new StringCollection();

                    foreach (var s in split)
                    {
                        var valor = s.Trim(' ', '(', ')', '-');

                        if (!string.IsNullOrEmpty(valor) && !string.IsNullOrWhiteSpace(valor))
                            collection.Add(valor);
                    }

                    if (collection.Count >= 5)
                    {
                        valores.Add(collection);
                        if (_buscarDadosShallow)
                            break;
                    }

                }

                index++;
            } while (!linha.Contains("______") && index < _linhas.Count);
        }
    }
}
