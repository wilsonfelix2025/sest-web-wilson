namespace SestWeb.Domain.Exportadores.Base
{
    public interface IExportadorBase
    {
        byte[] Exportar();
        byte[] ExportarRegistros();

    }
}
