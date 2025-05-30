using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using SestWeb.Domain.DTOs;

namespace SestWeb.Domain.Importadores.Leitores.SIGEO
{
    public class LeitorSapataSIGEO
    {
        public List<SapataDTO> Sapatas { get; private set; }
        private readonly LeitorHelperSIGEO _leitorGeral;
        private readonly IReadOnlyList<string> _linhas;
        private bool _buscarDadosShallow;

        public LeitorSapataSIGEO(IReadOnlyList<string> linhas, bool buscarDadosShallow)
        {
            _linhas = linhas;
            Sapatas = new List<SapataDTO>();
            _leitorGeral = new LeitorHelperSIGEO();
            _buscarDadosShallow = buscarDadosShallow;
            GetSapatas();
        }

        private void GetSapatas()
        {
            var index = _leitorGeral.EncontrarIdentificadorDeSeção("REVESTIMENTOS", _linhas);

            if (index == -1)
                return;

            const string padrao = @"\D*([-+]?[0-9]*\.?[0-9]*)\s*\D*\s*([-+]?[0-9]*\.?[0-9]*)\s*(\d+\/\d+)?";

            index += 2;

            string linha;
            do
            {
                linha = _linhas[index];

                if (!linha.Contains("DIAMETRO") && !string.IsNullOrEmpty(linha) && !linha.Contains("______"))
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

                        CriarSapatas(collection);

                        //se for para buscar shallow, achando a primeira sapata, pode sair do loop
                        if (_buscarDadosShallow)
                            break;

                    }
                }

                index++;
            } while (!linha.Contains("______") && index < _linhas.Count);
        }


        private void CriarSapatas(StringCollection collection)
        {
            var pm = collection[0].Trim();
            var diametro = collection[1].Trim();
            Sapatas.Add(new SapataDTO{
                Pm = pm,
                Diâmetro = diametro});
        }

    }
}
