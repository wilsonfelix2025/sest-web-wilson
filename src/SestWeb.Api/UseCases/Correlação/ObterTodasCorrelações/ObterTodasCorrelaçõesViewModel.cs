using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Correlação.ObterTodasCorrelações
{
    public class ObterTodasCorrelaçõesViewModel
    {
        public ObterTodasCorrelaçõesViewModel(Domain.Entities.Correlações.Base.Correlação corr)
        {
            Nome = corr.Nome;
            Descrição = corr.Descrição;
            NomeAutor = corr.Autor.Nome;
            ChaveAutor = corr.Autor.Chave;
            DataCriação = corr.Autor.DataCriação.ToShortDateString();
            Origem = corr.Origem.ToString();
            PerfilSaída = corr.PerfisSaída.Tipos[0];
            ExpressãoBruta = corr.Expressão.Bruta;
            Variáveis = corr.Expressão.Variáveis.Names;
            Constantes = corr.Expressão.Constantes.Names;
            PerfisEntrada = corr.Expressão.PerfisEntrada.Tipos;
        }

        public string Nome { get; }

        public string  Descrição { get; }

        public string NomeAutor { get; }

        public string DataCriação { get; }

        public string ChaveAutor { get; }

        public string Origem { get; }

        public string ExpressãoBruta { get; }

        public List<string> Variáveis { get; }

        public List<string> Constantes { get; }

        public List<string> PerfisEntrada { get; }

        public string PerfilSaída { get; }
    }
}
