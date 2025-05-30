using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Correlações.Dto
{
    public class CorrelaçãoDto
    {
        public CorrelaçãoDto(string nome, string perfilSaída, List<string> perfisEntrada, string chaveAutor, string origem)
        {
            Nome = nome;
            PerfilSaída = perfilSaída;
            PerfisEntrada = perfisEntrada;
            ChaveAutor = chaveAutor;
            Origem = origem;
        }

        public string Nome { get; }

        public string PerfilSaída { get; }

        public List<string> PerfisEntrada { get; }

        public string ChaveAutor { get; }

        public string Origem { get; }
    }
}
