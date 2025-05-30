
using System.Collections.Generic;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Api.UseCases.Importação.ImportarDados
{
    public class ImportarDadosModel
    {
        public ImportarDadosModel(string mensagem,List<PerfilBase> perfis)
        {
            Mensagem = mensagem;
            Perfis = perfis;
        }

        public string Mensagem { get; }

        public List<PerfilBase> Perfis { get; }
    }
}
