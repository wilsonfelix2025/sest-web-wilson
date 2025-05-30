using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Enums;

namespace SestWeb.Domain.Importadores.Leitores.SIGEO
{
    public class LeitorEstratigrafiaSIGEO
    {
        public EstratigrafiaDTO Estratigrafia { get; private set; }
        private readonly LeitorHelperSIGEO _leitorGeral;
        private readonly IReadOnlyList<string> _linhas;
        private bool _buscarDadosShallow;

        public LeitorEstratigrafiaSIGEO(IReadOnlyList<string> linhas, bool buscarDadosShallow)
        {
            _linhas = linhas;
            Estratigrafia = new EstratigrafiaDTO();
            _leitorGeral = new LeitorHelperSIGEO();
            _buscarDadosShallow = buscarDadosShallow;
            GetEstratigrafia();
        }

        private void GetEstratigrafia()
        {
            CarregarLitoEstratigrafias();
            CarregarCronoEstratigrafias();
        }

        private void CarregarLitoEstratigrafias()
        {
            int index = _leitorGeral.EncontrarIdentificadorDeSeção("UNIDADES LITOESTRATIGRAFICAS", _linhas);

            FiltrarDados(index, TipoDadosEstratigráficosEnum.LitoEstratigráficos);
        }

        private void CarregarCronoEstratigrafias()
        {
            int index = _leitorGeral.EncontrarIdentificadorDeSeção("UNIDADES CRONOESTRATIGRAFICAS", _linhas);

            FiltrarDados(index, TipoDadosEstratigráficosEnum.CronoEstratigráficos);
        }

        private void FiltrarDados(int index, TipoDadosEstratigráficosEnum tipoDados)
        {
            if (index == -1)
                return;

            index += 2;

            //  Exemplo de linha do arquivo
            //                      Q       UNIDADE TOPO(Prof    Cota)  Q  BASE(Prof   Cota)  Q  MET. INTERP  DATA INTERP FONTE INTERPRETACAO   COMENTARIO
            //                      X M     GERIBA       280.0  -252.0  D       740.0  -712.0    LITO         24/10/1994  DADO NAO REVISTO

            // Encontrar padrão de unidade:  X M     GERIBA
            var padraoUnidade = @"([a-zA-Z]\s*)([a-zA-Z]?)\s+(\D+)";

            // Encontrar padrão de valores de topo e base:  280.0 -252.0  740.0  -712.0
            var padraoTopoBase =
                @"\D*([-+]?[0-9]*\.?[0-9]*)\s*([-+]?[0-9]*\.?[0-9]*)\s*\D*\s*([-+]?[0-9]*\.?[0-9]*)\s*([-+]?[0-9]*\.?[0-9]*)";


            string linha;
            do
            {
                linha = _linhas[index];
                if (!linha.Contains("UNIDADE") && !string.IsNullOrEmpty(linha) && !linha.Contains("______"))
                {
                    linha = Regex.Replace(linha, @"(\d+\/\d+\/\d+.+)|(\d+\-\d+\-\d+.+)", string.Empty);
                    linha = Regex.Replace(linha, @"\s{1,}", " ").Trim();

                    var collection = new StringCollection();

                    Match m = Regex.Match(linha, padraoUnidade);

                    if (m.Success)
                    {
                        if (tipoDados == TipoDadosEstratigráficosEnum.LitoEstratigráficos)
                            collection.Add(m.Groups[2].Value.Trim());
                        else
                            collection.Add("CR");

                        collection.Add(m.Groups[3].Value.Trim());


                        Match valoresMatch = Regex.Match(linha, padraoTopoBase);

                        if (valoresMatch.Success)
                        {
                            for (int i = 1; i < valoresMatch.Groups.Count; i++)
                            {
                                var valor = valoresMatch.Groups[i].Value.Trim().Trim(new char[] { ' ', '(', ')', '-' });
                                if (!string.IsNullOrEmpty(valor) && !string.IsNullOrWhiteSpace(valor))
                                    collection.Add(valor);
                            }
                        }
                    }

                    if (collection.Count >= 3)
                    {
                        CriarEstratigrafias(collection);

                        if (_buscarDadosShallow)
                            break;
                    }
                }

                index++;
            } while (!linha.Contains("______") && index < _linhas.Count);
        }

        private void CriarEstratigrafias(StringCollection estratigrafia)
        {
            if (estratigrafia == null || estratigrafia.Count <= 0)
                return;

            var basePm = "";
            var tipo = estratigrafia[0].ToUpper();
            var sigla = estratigrafia[1];

            var topoPm = estratigrafia[2].Trim();

            if (estratigrafia.Count > 4)
                basePm = estratigrafia[4].Trim();
            else
                basePm = "";

            if (Estratigrafia.Itens.ContainsKey(tipo))
            {
                var ultimoItemDoTipo = Estratigrafia.Itens[tipo].LastOrDefault();

                if (ultimoItemDoTipo != null && ultimoItemDoTipo.BasePm != "" && ultimoItemDoTipo.BasePm != topoPm)
                {
                    var itemVazio = new ItemEstratigrafiaDTO(ultimoItemDoTipo.BasePm, "", "", "");
                    Estratigrafia.Itens[tipo].Add((itemVazio));
                }
            }

            var lista = new List<ItemEstratigrafiaDTO>();

            var item = new ItemEstratigrafiaDTO(topoPm, sigla, "", "", basePm);

            if (Estratigrafia.Itens.ContainsKey(tipo))
            {
                Estratigrafia.Itens[tipo].Add(item);
            }
            else
            {
                lista.Add(item);
                Estratigrafia.Itens.Add(tipo, lista);
            }
        }

    }
}
