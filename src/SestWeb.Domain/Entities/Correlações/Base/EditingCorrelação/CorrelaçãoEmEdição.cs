namespace SestWeb.Domain.Entities.Correlações.Base.EditingCorrelação
{
    public class CorrelaçãoEmEdição
    {
        public CorrelaçãoEmEdição(string origem)
        {
            Origem = origem;
        }

        public string Origem { get; }
    }
}
