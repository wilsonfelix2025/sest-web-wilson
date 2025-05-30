using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SestWeb.Api.UseCases.Correlação.ObterCorrelaçõesPorTipo
{
    public class ObterCorrelaçõesPorTipoViewModel
    {
        public ObterCorrelaçõesPorTipoViewModel(Domain.Entities.Correlações.Base.Correlação corr)
        {
            Nome = corr.Nome;
            PerfilSaída = corr.PerfisSaída.Tipos[0];
            ChaveAutor = corr.Autor.Chave;
            Origem = corr.Origem.ToString();
        }

        public string Nome { get; }

        public string PerfilSaída { get; }

        public string ChaveAutor { get; }

        public string Origem { get; }
    }
}
