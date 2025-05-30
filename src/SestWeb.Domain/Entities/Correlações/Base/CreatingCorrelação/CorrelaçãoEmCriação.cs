namespace SestWeb.Domain.Entities.Correlações.Base.CreatingCorrelação
{
    public class CorrelaçãoEmCriação
    {
        public string Nome { get; }
        public string NomeAutor { get; }
        public string ChaveAutor { get; }
        public string DataCriação { get; }
        public string Descrição { get; }
        public string Origem { get; }
        public string Expressão { get; }

        public CorrelaçãoEmCriação(string nome, string nomeAutor, string chaveAutor, string dataCriação, string descrição, string origem, string expressão)
        {
            Nome = nome;
            NomeAutor = nomeAutor;
            ChaveAutor = chaveAutor;
            DataCriação = dataCriação;
            Descrição = descrição;
            Origem = origem;
            Expressão = expressão;
        }
    }
}
