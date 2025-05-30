using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.UsuárioUseCases.EsqueceuSenha
{
    public class EsqueceuSenhaUseCaseOutput : UseCaseOutput<EsqueceuSenhaUseCaseStatus>
    {
        public string CodigoReset { get; private set; }

        public static EsqueceuSenhaUseCaseOutput UsuarioNaoEncontrado()
        {
            return new EsqueceuSenhaUseCaseOutput()
            {
                Mensagem = "Usuário não encontrado ou não possui e-mail confirmado!",
                Status = EsqueceuSenhaUseCaseStatus.EmailNaoEnviado
            };
        }

        public static EsqueceuSenhaUseCaseOutput EmailEnviado(string codigoReset)
        {
            return new EsqueceuSenhaUseCaseOutput()
            {
                Mensagem = "E-mail de recuperação de senha enviado!",
                Status = EsqueceuSenhaUseCaseStatus.EmailEnviado,
                CodigoReset = codigoReset
            };
        }
    }
}
