using System;

namespace SestWeb.Domain.Entities.Correlações.AutorCorrelação
{
    public interface IAutorFactory
    {
        IAutor CreateAutor(string nome, string chave, DateTime dataCriação);
    }
}
