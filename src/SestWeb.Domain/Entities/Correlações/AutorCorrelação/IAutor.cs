using System;

namespace SestWeb.Domain.Entities.Correlações.AutorCorrelação
{
    public interface IAutor
    {
        string Nome { get; }
        string Chave { get; }
        DateTime DataCriação { get; }
    }
}
