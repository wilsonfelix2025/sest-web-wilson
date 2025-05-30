using SestWeb.Domain.Importadores.SestTR1.Utils;
using SestWeb.Domain.Importadores.Shallow;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SestWeb.Domain.Importadores.SestTR1.Shallow
{
    public class LeitorShallowSestTR1
    {
        private StreamReader _streamReader;
        public LeitorShallowLitologiaTR1 LeitorLitologia { get; private set; } = new LeitorShallowLitologiaTR1();
        public LeitorShallowTrajetóriaTR1 LeitorTrajetória { get; private set; } = new LeitorShallowTrajetóriaTR1();
        public LeitorShallowPerfisTR1 LeitorPerfis { get; private set; } = new LeitorShallowPerfisTR1();

        public LeitorShallowSestTR1(string caminho)
        {
            FileStream fileStream = File.OpenRead(caminho);
            GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            _streamReader = new StreamReader(gzipStream);
        }

        public DadosLidos LerDados()
        {
            var arquivoTemDadosGerais = false;
            var temRegistros = false;

            while (!_streamReader.EndOfStream)
            {
                var linha = _streamReader.ReadLine();

                if (LeitorHelperTR1.ÉDataset(linha))
                {
                    if (LeitorHelperTR1.ÉLinhaDeLitologia(linha))
                    {
                        LeitorLitologia.AdicionarLitologia(linha);
                    }
                    else if (LeitorHelperTR1.ÉLinhaDeTrajetória(linha))
                    {
                        LeitorTrajetória.AdicionarTrajetória(linha);
                    }
                    else
                    {
                        LeitorPerfis.AdicionarPerfil(linha);
                    }
                }
                else if (LeitorHelperTR1.ÉDadosGerais(linha))
                {
                    arquivoTemDadosGerais = true;
                }
                else if (LeitorHelperTR1.ÉRegistros(linha))
                {
                    temRegistros = true;
                }
            }

            try
            {
                var retorno = new DadosLidos {
                    TemSapatas = false,         // SEST TR1 não tem dados de sapatas
                    TemObjetivos = false,       // SEST TR1 não tem dados de objetivos
                    TemEstratigrafia = false,   // SEST TR1 não tem dados de estratigrafia
                    TemDadosGerais = arquivoTemDadosGerais, 
                    TemTrajetória = LeitorTrajetória.Trajetórias.Count > 0, 
                    TemLitologia = LeitorLitologia.Litologias.Count() > 0,
                    Litologias = LeitorLitologia.Litologias, 
                    Perfis = LeitorPerfis.Perfis,
                    Trajetórias = LeitorTrajetória.Trajetórias,
                    TemEventos = temRegistros, //arquivo SEST TR1 não faz distinção de eventos e registros
                    TemRegistros = temRegistros
                };

                return retorno;
            }
            catch (Exception e)
            {
                throw new Exception($"LeitorShallowSest5 - Não foi possível ler dados - {e.Message}");
            }
        }
    }
}
