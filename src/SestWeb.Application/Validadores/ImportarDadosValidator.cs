namespace SestWeb.Application.Validadores
{
    public class ImportarDadosValidator
    {
        public string Result = string.Empty;

        public string ValidarDadosParaImportarNãoNulo<T>(T obj)
        {
            if (obj == null)
                Result = "Dados para importação de planilha não preenchidos";

            return Result;
        }

    }
}
