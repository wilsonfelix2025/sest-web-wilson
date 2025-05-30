using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LZMA = SevenZip.Compression.LZMA;

namespace SestWeb.Domain.Importadores.Shallow.SestTR2
{
    class LeitorHelperTR2
    {
        public static Dictionary<string, string> nomesPerfis = new Dictionary<string, string>();
        public static Dictionary<string, string> nomesLitologias = new Dictionary<string, string>();


        public static StreamReader AbrirArquivoSestTR2(string caminhoArquivo)
        {
            var caminhoPrimeiraDescompressão = DescomprimirPrimeiraVez(caminhoArquivo);
            Console.WriteLine("Primeira descompressão: OK.");
            var conteudoSegundaDescompressão = DescomprimirSegundaVez(caminhoPrimeiraDescompressão);
            Console.WriteLine("Segunda descompressão: OK.");
            conteudoSegundaDescompressão.Position = 0;

            return new StreamReader(conteudoSegundaDescompressão, Encoding.UTF8);
        }

        public static string DescomprimirPrimeiraVez(string caminhoInput)
        {
            string outfilePath = Path.GetTempFileName();
            outfilePath = Path.ChangeExtension(outfilePath, ".xsrt2");

            LZMA.Decoder coder = new LZMA.Decoder();
            using (FileStream input = new FileStream(caminhoInput, FileMode.Open))
            using (FileStream output = new FileStream(outfilePath, FileMode.Create))
            {
                try
                {
                    // Read the decoder properties
                    byte[] properties = new byte[5];
                    input.Read(properties, 0, 5);

                    // Read in the decompress file size.
                    byte[] fileLengthBytes = new byte[8];
                    input.Read(fileLengthBytes, 0, 8);
                    long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                    coder.SetDecoderProperties(properties);
                    coder.Code(input, output, input.Length, fileLength, null);
                    output.Flush();
                }
                catch (Exception)
                {
                    File.Delete(output.Name);
                }
            }

            return outfilePath;
        }

        public static Stream DescomprimirSegundaVez(string caminhoInput)
        {
            var streamSaída = new MemoryStream();

            try
            {
                using (var gzipStream = new GZipStream(File.OpenRead(caminhoInput), CompressionMode.Decompress))
                {
                    gzipStream.CopyTo(streamSaída);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro ao descomprimir via GZIP");
            }

            return streamSaída;
        }
         
        public static string ObterValorTag(string linha)
        {
            if (linha.Contains("/>"))
            {
                return "";
            }

            linha = linha.Trim();
            linha = linha.Replace("\n", string.Empty);
            return linha.Split('>')[1].Split('<')[0];
        }

        public static string ObterNomeDataset(XElement elemento)
        {
            var assemblyQualifiedName = elemento.Attribute("AssemblyQualifiedName").Value;
            var nomeCompletoDataset = assemblyQualifiedName.Split(",")[0];
            return nomeCompletoDataset.Split(".").Last();
        }

        public static string ObterAtributo(string linha, string atributo)
        {
            var partesSplitAtributo = linha.Trim().Split($" {atributo}=\"");

            if (partesSplitAtributo.Length >= 2)
            {
                return partesSplitAtributo[1].Split("\"")[0];
            }

            return null;
        }

        public static string CompletarTag(string linha, string nomeTag)
        {
            return linha + $"</{nomeTag}>";
        }

        public static bool ÉDataset(string linha)
        {
            return ÉLinhaDeTrajetória(linha) || ÉLinhaDeLitologia(linha) || ÉLinhaDePerfil(linha) || ÉLinhaDeRegistro(linha) || ÉLinhaDeSapata(linha) || ÉLinhaDeObjetivo(linha) || ÉLinhaDeEstratigrafia(linha);
        }

        public static bool ÉDadosGerais(string linha)
        {
            return linha.Trim().StartsWith("<d4p1:DadosGerais");
        }

        public static bool ÉFimDeDataset(string linha)
        {
            linha = linha.Trim();
            return linha.Contains("</d6p1:_pontosTrajetória") || ÉFimDePerfil(linha) || ÉFimDeLitologia(linha) || ÉFimDeSapata(linha) || ÉFimDeObjetivo(linha) || ÉFimDeEstratigrafia(linha) || linha.Contains("</d4p1:_registrosDePerfuração>");
        }

        public static bool ÉFimDeDadosGerais(string linha)
        {
            return linha.Trim().StartsWith("</d4p1:Geometria");
        }

        public static bool ÉFimDePerfil(string linha)
        {
            return linha.Trim().Contains("</d5p1:Perfil");
        }

        public static bool ÉPontoDeTrajetória(string linha)
        {
            return linha.Trim().StartsWith("<TrajectoryPoint ");
        }

        public static bool ÉPontoGenérico(string linha)
        {
            return linha.Trim().StartsWith("<Point ");
        }

        public static bool ÉLinhaDeTrajetória(string linha)
        {
            return linha.Trim().Contains("<d6p1:_pontosTrajetória");
        }

        public static bool ÉLinhaDeRegistro(string linha)
        {
            return linha.Trim().Contains("<d5p1:_registrosDePerfuração");
        }

        public static bool ÉLinhaComNomeDoPoço(string linha)
        {
            return linha.Trim().Contains("<d4p1:Nome");
        }

        public static bool ÉLinhaTipoPoço(string linha)
        {
            return linha.Trim().Contains("<d4p1:Tipo");
        }

        public static bool ÉLinhaDeLitologia(string linha)
        {
            linha = linha.Trim();
            return (linha.Contains(".LitologiaEntity") && linha.Contains(":Litologia")) 
                || (linha.Contains(":LitologiaAdaptada") && linha.Contains("z:Id"))
                || (linha.Contains("<d5p1:Litologia") && linha.Contains("z:Id"));
        }

        public static bool ÉLinhaDePerfil(string linha)
        {
            return linha.Trim().Contains("<d5p1:Perfil");
        }

        public static bool ÉLinhaDeSapata(string linha)
        {
            return linha.Trim().Contains("<d5p1:Sapata");
        }

        public static bool ÉLinhaDeObjetivo(string linha)
        {
            return linha.Trim().Contains("<d5p1:Objetivo");
        }

        public static bool ÉLinhaDeEstratigrafia(string linha)
        {
            return linha.Trim().Contains("<d5p1:Estratigrafia");
        }

        public static bool ÉFimDeEstratigrafia(string linha)
        {
            return linha.Trim().Contains("</d5p1:Estratigrafia");
        }

        public static bool ÉFimDeObjetivo(string linha)
        {
            return linha.Trim().Contains("</d5p1:Objetivo");
        }

        public static bool ÉFimDeSapata(string linha)
        {
            return linha.Trim().Contains("</d5p1:Sapata");
        }

        public static bool ÉComeçoDePoço(string linha)
        {
            return linha.Trim().Contains("<d2p1:KeyValueOfguidPoço");
        }

        public static bool ÉFimDePoço(string linha)
        {
            return linha.Trim().Contains("</d2p1:KeyValueOfguidPoço");
        }

        public static bool ÉFimDePoçoArquivo(string linha)
        {
            return linha.Trim().Contains("</_poços>");
        }

        internal static bool ÉFimDeLitologia(string linha)
        {
            return (linha.Trim().Contains("</d4p1:Litologia"))
                 ||(linha.Trim().Contains("</d5p1:Litologia>"));
        }
    }
}
