using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoTensõesInSitu.CriarPerfilRelaçãoTensões
{
    public class CriarPerfilRelaçãoTensõesOutput : UseCaseOutput<CriarPerfilRelaçãoTensõesStatus>
    {
        public CriarPerfilRelaçãoTensõesOutput()
        {

        }

        public PerfilBase Perfil { get; set; }

        public static CriarPerfilRelaçãoTensõesOutput PerfilCriado(PerfilBase perfil)
        {
            return new CriarPerfilRelaçãoTensõesOutput
            {
                Status = CriarPerfilRelaçãoTensõesStatus.PerfilCriado,
                Mensagem = "Perfil criado com sucesso",
                Perfil = perfil
            };
        }

        public static CriarPerfilRelaçãoTensõesOutput PerfilNãoCriado(string msg)
        {
            return new CriarPerfilRelaçãoTensõesOutput
            {
                Status = CriarPerfilRelaçãoTensõesStatus.PerfilNãoCriado,
                Mensagem = $"Não foi possível criar perfil. {msg}"                
            };
        }
    }
}
