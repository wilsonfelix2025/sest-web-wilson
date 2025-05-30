using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Importadores.Leitores.SIGEO
{
    public class LeitorObjetivosSIGEO
    {
        public List<ObjetivoDTO> Objetivos { get; private set; }
        private readonly LeitorHelperSIGEO _leitorGeral;
        private readonly IReadOnlyList<string> _linhas;
        private readonly bool _buscarDadosShallow;
        
        public LeitorObjetivosSIGEO(IReadOnlyList<string> linhas, bool buscarDadosShallow)
        {
            _linhas = linhas;
            Objetivos = new List<ObjetivoDTO>();
            _leitorGeral = new LeitorHelperSIGEO();
            _buscarDadosShallow = buscarDadosShallow;
            GetObjetivos();
        }

        private void GetObjetivos()
        {
            var index = _leitorGeral.EncontrarIdentificadorDeSeção("OBJETIVOS DO POCO", _linhas);
            if (index == -1)
                return;

            const string padrao = @"\D*([-+]?[0-9]*\.?[0-9]*)\s*\D*\s*([-+]?[0-9]*\.?[0-9]*)\s*(\d+\/\d+)?";

            index += 2;

            string linha;

            do
            {
                linha = _linhas[index];

                if (linha.Contains("TIPO") && !string.IsNullOrEmpty(linha) && !linha.Contains("______"))
                {
                    //Retirar parenteses "()" e o que estiver dentro
                    linha = Regex.Replace(linha, @"\(([^)]+)\)", string.Empty);
                    linha = Regex.Replace(linha, @"\s{1,}", " ").Trim();

                    Match m = Regex.Match(linha, padrao);

                    if (m.Success)
                    {
                        var collection = new StringCollection();

                        collection.Add(m.Groups[1].Value.Trim());

                        if (m.Groups[3].Success)
                        {
                            var valor = m.Groups[2].Value.Trim() + " " + m.Groups[3].Value.Trim();
                            collection.Add(valor);
                        }
                        else
                            collection.Add(m.Groups[2].Value.Trim());

                        CriarObjetivos(collection);

                        //se for para buscar shallow, achando a primeira sapata, pode sair do loop
                        if (_buscarDadosShallow)
                            break;

                    }
                }

                index++;
            } while (!linha.Contains("______") && index < _linhas.Count);

        }

        private void CriarObjetivos(StringCollection collection)
        {
            var pm = collection[0].Trim();
            var tipoObjetivo = collection[1].Trim();
            Objetivos.Add(new ObjetivoDTO(pm, tipoObjetivo));
        }
    }
}
