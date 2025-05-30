using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.UsuárioUseCases.TrocarSenha
{
    public class TrocarSenhaUseCaseOutput : UseCaseOutput<TrocarSenhaUseCaseStatus>
    {
        public static TrocarSenhaUseCaseOutput SenhaTrocada()
        {
           return new TrocarSenhaUseCaseOutput()
           {
               Mensagem = "Senha trocada com sucesso!",
               Status = TrocarSenhaUseCaseStatus.SenhaTrocada
           };
        }

        public static TrocarSenhaUseCaseOutput SenhaNãoTrocada()
        {
            return new TrocarSenhaUseCaseOutput()
            {
                Mensagem = "Senha não pôde ser trocada!",
                Status = TrocarSenhaUseCaseStatus.SenhaNãoTrocada
            }; 
        }
    }
}
