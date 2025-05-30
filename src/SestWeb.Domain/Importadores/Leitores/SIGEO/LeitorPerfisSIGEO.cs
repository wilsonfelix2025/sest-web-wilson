using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.DTOs.Importação;
using SestWeb.Domain.Entities.PontosEntity;

namespace SestWeb.Domain.Importadores.Leitores.SIGEO
{
    public class LeitorPerfisSIGEO
    {
        public PerfilDTO Perfil { get; private set; }
        private readonly List<PerfilParaImportarDTO> _perfisSelecionados;
        private readonly LeitorHelperSIGEO _leitorGeral;
        private readonly IReadOnlyList<string> _linhas;
        private bool _buscarDadosShallow;
        private double _baseSedimentos = 0.0;

        public LeitorPerfisSIGEO(IReadOnlyList<string> linhas, bool buscarDadosShallow, List<PerfilParaImportarDTO> perfisSelecionados, double baseSedimentos)
        {
            _linhas = linhas;
            Perfil = new PerfilDTO();
            _leitorGeral = new LeitorHelperSIGEO();
            _buscarDadosShallow = buscarDadosShallow;
            _perfisSelecionados = perfisSelecionados;
            _baseSedimentos = baseSedimentos;
            GetPerfilDiâmetroDaBroca();
        }

        private void GetPerfilDiâmetroDaBroca()
        {
            if (_buscarDadosShallow == false && (_perfisSelecionados == null || _perfisSelecionados.Count == 0))
                Perfil = null;
            else
            {
                Perfil.Nome = "DIAM_BROCA";
                Perfil.Mnemonico = "DIAM_BROCA";
                Perfil.Descrição = "";
                Perfil.Grupo = "";
                CarregarInformações();
            }
        }

        private void CarregarInformações()
        {
            int index = _leitorGeral.EncontrarIdentificadorDeSeção("DIAMETROS DAS FASES DO POCO", _linhas);

            if (index == -1)
                return;

            string padrao = @"([-+]?[0-9]*\.?[0-9]*)\s*\D*\s*([-+]?[0-9]*\.?[0-9]*)\s*(\d+\/\d+)?";

            index += 2;

            string linha;
            var primeiraVez = true;
            var ultimoValor = "";
            var collection = new StringCollection();
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
                        var valor = string.Empty;
                        if (primeiraVez)
                        {
                            collection.Add(_baseSedimentos.ToString());
                            if (m.Groups[3].Success)
                            {
                                var valorEmDouble = double.Parse(m.Groups[2].Value.Trim(), CultureInfo.InvariantCulture) + ConverterParaDecimal(m.Groups[3].Value.Trim());
                                valor = valorEmDouble.ToString(CultureInfo.InvariantCulture);
                                collection.Add(valor);
                            }
                            else
                            {
                                valor = m.Groups[2].Value.Trim();
                                collection.Add(valor);
                            }

                            CriarPontos(collection);

                           
                            collection = new StringCollection();
                            collection.Add(m.Groups[1].Value.Trim());

                            if (_buscarDadosShallow)
                                break;

                            primeiraVez = false;
                            ultimoValor = valor;
                        }
                        else
                        {
                            
                            if (m.Groups[3].Success)
                            {
                                var valorEmDouble = double.Parse(m.Groups[2].Value.Trim(), CultureInfo.InvariantCulture) + ConverterParaDecimal(m.Groups[3].Value.Trim());
                                valor = valorEmDouble.ToString(CultureInfo.InvariantCulture);
                                collection.Add(valor);
                            }
                            else
                            {
                                valor = m.Groups[2].Value.Trim();
                                collection.Add(valor);
                            }

                            CriarPontos(collection);
                            ultimoValor = valor;
                            collection = new StringCollection();
                            collection.Add(m.Groups[1].Value.Trim());

                        }
                    }
                }

                index++;
            } while (!linha.Contains("______") && index < _linhas.Count);

            collection.Add(ultimoValor);
            CriarPontos(collection);
        }

        private double ConverterParaDecimal(string strValor)
        {
            string[] split = strValor.Split('/');

            var valorNumero = double.Parse(split[0], CultureInfo.InvariantCulture) / double.Parse(split[1], CultureInfo.InvariantCulture);

            return valorNumero;
        }

        private void CriarPontos(StringCollection collection)
        {
            if (collection == null || collection.Count <= 0)
                return;

            var pm = collection[0].Trim();
            var diametro = collection[1].Trim();            

            var ponto = new PontoDTO
            {
                Pm = pm,
                Origem = OrigemPonto.Importado,
                Pv = null,
                Valor = diametro
            };

            Perfil.PontosDTO.Add(ponto);

        }
    }
}
