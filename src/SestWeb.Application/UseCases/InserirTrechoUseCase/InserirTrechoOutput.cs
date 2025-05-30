using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.InserirTrechoUseCase
{
    public class InserirTrechoOutput : UseCaseOutput<InserirTrechoStatus>
    {
        public InserirTrechoOutput()
        {
            
        }

        public PerfilBase Perfil { get; set; }
        public static InserirTrechoOutput InserirTrechoCriadoComSucesso(PerfilBase perfil)
        {
            return new InserirTrechoOutput
            {
                Status = InserirTrechoStatus.InserirTrechoComSucesso,
                Mensagem = "Trecho inserido com sucesso.",
                Perfil = perfil
            };
        }

        public static InserirTrechoOutput InserirTrechoComFalhaDeValidação(string mensagem)
        {
            return new InserirTrechoOutput
            {
                Status = InserirTrechoStatus.InserirTrechoComFalhaDeValidação,
                Mensagem = $"Não foi possível inserir trecho. {mensagem}"
            };
        }

        public static InserirTrechoOutput InserirTrechoComFalha(string mensagem)
        {
            return new InserirTrechoOutput
            {
                Status = InserirTrechoStatus.InserirTrechoComFalha,
                Mensagem = $"Não foi possível inserir trecho inicial. {mensagem}"
            };
        }

    }
}
