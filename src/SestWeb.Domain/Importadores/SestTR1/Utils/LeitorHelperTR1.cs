using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace SestWeb.Domain.Importadores.SestTR1.Utils
{
    class LeitorHelperTR1
    {
        public static StreamReader AbrirArquivoSestTR1 (string caminhoArquivo)
        {
            FileStream fileStream = File.OpenRead(caminhoArquivo);
            GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            return new StreamReader(gzipStream);
        }
        public static string ObterNomeDataset(string linha)
        {
            linha = CompletarTag(linha.Trim(), "DatasetItem");
            var elem = XElement.Parse(linha);
            return ObterNomeDataset(elem);
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
            return linha.Trim().StartsWith("<DatasetItem AssemblyQualifiedName=\"Model.Common.Datasets.");
        }

        public static bool ÉDadosGerais(string linha)
        {
            return linha.Trim().StartsWith("<WellData>");
        }

        public static bool ÉRegistros(string linha)
        {
            return linha.Trim().StartsWith("<GraphicsRegisterOfDrilling>");
        }
        
        public static bool ÉFimDeDataset(string linha)
        {
            return linha.Trim().StartsWith("</DatasetItem>");
        }

        public static bool ÉFimDeDadosGerais(string linha)
        {
            return linha.Trim().StartsWith("</WellData>");
        }

        public static bool ÉFimDeRegistros(string linha)
        {
            return linha.Trim().StartsWith("</GraphicsRegisterOfDrilling>");
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
            return linha.Trim().StartsWith("<DatasetItem AssemblyQualifiedName=\"Model.Common.Datasets.ProjTrajectory");
        }

        public static bool ÉLinhaDeLitologia(string linha)
        {
            return linha.Trim().StartsWith("<DatasetItem AssemblyQualifiedName=\"Model.Common.Datasets.Lithology");
        }
    }
}
