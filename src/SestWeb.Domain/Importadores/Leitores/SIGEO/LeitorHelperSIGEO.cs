using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SestWeb.Domain.Importadores.Leitores.SIGEO
{
    public class LeitorHelperSIGEO
    {

        private Dictionary<string, string> _dados = new Dictionary<string, string>();

        public LeitorHelperSIGEO()
        {
            
        }
        public int EncontrarIdentificadorDeSeção(string identificador, IReadOnlyList<string> linhas)
        {
            var pattern = $@"\b{identificador}\s?\B";
            var regex = new Regex(pattern);

            for (var i = 0; i < linhas.Count; i++)
            {
                string linha = linhas[i];

                if (regex.IsMatch(linha))
                {
                    return i;
                }
            }

            return -1;
        }

        public List<int> EncontrarIdentificadoresDeSeção(string identificador, IReadOnlyList<string> linhas)
        {
            var pattern = $@"\b{identificador}\b";
            var regex = new Regex(pattern);
            List<int> identificadores = new List<int>();

            for (int i = 0; i < linhas.Count; i++)
            {
                string linha = linhas[i];

                if (regex.IsMatch(linha))
                {
                    identificadores.Add(i);
                }
            }

            return identificadores;
        }

        public void CarregarDados(int index, IReadOnlyList<string> linhas)
        {
            index += 2;

            string linha;
            do
            {
                linha = linhas[index];
                string[] valores = linha.Split(':');

                if (valores.Length >= 2)
                {
                    string id = valores[0].Trim();
                    string dado;

                    if (!_dados.TryGetValue(id, out dado))
                    {
                        dado = valores[1].Trim();
                        _dados.Add(id, dado);
                    }
                }

                index++;
            } while (!linha.Contains("______") && index < linhas.Count);
        }

        public string ObterDado(string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                return string.Empty;

            string dado = string.Empty;

            if (!_dados.TryGetValue(id, out dado))
            {
                var dados = _dados.Keys.ToList().Where(d => d.Contains(id));
                string newId = dados.FirstOrDefault();
                if (string.IsNullOrEmpty(newId) || string.IsNullOrWhiteSpace(newId))
                    return string.Empty;

                _dados.TryGetValue(newId, out dado);
            }

            return dado;
        }

        public string ObterDadoDireto(string id, IReadOnlyList<string> linhas, bool utilizarEspaçoEmBrancoComoSeparador = false)
        {
            var dado = string.Empty;

            var index = EncontrarIdentificadorDeSeção(id, linhas);

            if (index != -1)
            {
                if (utilizarEspaçoEmBrancoComoSeparador == false)
                {
                    var linha = linhas[index];
                    var valores = linha.Split(':');
                    dado = valores[1].Trim();
                }
                else
                {
                    var linha = linhas[index];
                    var valores = linha.Split("  ", StringSplitOptions.RemoveEmptyEntries);
                    dado = valores[1].Trim();
                }
            }

            return dado;
        }
    }

   
}
