using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.UsuárioUseCases.ResetarSenha
{
    public class ResetarSenhaUseCaseOutput : UseCaseOutput<ResetarSenhaUseCaseStatus>
    {
        public static ResetarSenhaUseCaseOutput SenhaResetada()
        {
            return new ResetarSenhaUseCaseOutput()
            {
                Mensagem = "Senha resetada com sucesso!",
                Status = ResetarSenhaUseCaseStatus.SenhaResetada
            };
        }

        public static ResetarSenhaUseCaseOutput SenhaNãoResetada()
        {
            return new ResetarSenhaUseCaseOutput()
            {
                Mensagem = "A senha não pôde ser resetada!",
                Status = ResetarSenhaUseCaseStatus.SenhaNãoResetada
            };
        }
    }
}
