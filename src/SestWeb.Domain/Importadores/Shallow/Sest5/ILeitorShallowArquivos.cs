namespace SestWeb.Domain.Importadores.Shallow.Sest5
{
    public interface ILeitorShallowArquivos
    {
        RetornoDTO LerArquivo(TipoArquivo tipoArquivo, string caminho, object obj = null);
    }
}
