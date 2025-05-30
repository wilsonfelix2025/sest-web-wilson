using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Correlações.Base;

namespace SestWeb.Application.UseCases.CorrelaçãoUseCases.ObterCorrelaçãoPeloNome
{
    public class ObterCorrelaçãoPeloNomeOutput : UseCaseOutput<ObterCorrelaçãoPeloNomeStatus>
    {
        private ObterCorrelaçãoPeloNomeOutput(){ }

        public string Nome { get; private set; }

        public string PerfilSaída { get; private set; }

        public string Expressão { get; private set; }

        public string ChaveAutor { get; private set; }

        public string Descrição { get; private set; }

        public static ObterCorrelaçãoPeloNomeOutput CorrelaçãoObtida(Correlação correlação)
        {
            return new ObterCorrelaçãoPeloNomeOutput
            {
                Nome = correlação.Nome,
                PerfilSaída = correlação.PerfisSaída.Tipos[0],
                Expressão = correlação.Expressão.Bruta,
                ChaveAutor = correlação.Autor.Chave,
                Descrição = correlação.Descrição,
                Status = ObterCorrelaçãoPeloNomeStatus.CorrelaçãoObtida,
                Mensagem = "Correlação obtida com sucesso."
            };
        }

        public static ObterCorrelaçãoPeloNomeOutput CorrelaçãoNãoEncontrada(string nome)
        {
            return new ObterCorrelaçãoPeloNomeOutput
            {
                Status = ObterCorrelaçãoPeloNomeStatus.CorrelaçãoNãoEncontrada,
                Mensagem = $"Não foi possível encontrar correlação com nome {nome}."
            };
        }

        public static ObterCorrelaçãoPeloNomeOutput CorrelaçãoNãoObtida(string mensagem)
        {
            return new ObterCorrelaçãoPeloNomeOutput
            {
                Status = ObterCorrelaçãoPeloNomeStatus.CorrelaçãoNãoObtida,
                Mensagem = $"Não foi possível obter correlação. {mensagem}"
            };
        }

        public static ObterCorrelaçãoPeloNomeOutput PoçoNãoEncontrado(string idPoço)
        {
            return new ObterCorrelaçãoPeloNomeOutput
            {
                Status = ObterCorrelaçãoPeloNomeStatus.PoçoNãoEncontrado,
                Mensagem = $"A correlação não é uma correlação do sistema. Não foi possível encontrar poço com id {idPoço}."
            };
        }
    }
}
