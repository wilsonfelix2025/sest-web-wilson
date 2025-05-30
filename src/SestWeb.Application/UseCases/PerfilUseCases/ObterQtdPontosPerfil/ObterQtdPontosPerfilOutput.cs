using SestWeb.Application.Helpers;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisParaTrecho;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterQtdPontosPerfil
{
    public class ObterQtdPontosPerfilOutput : UseCaseOutput<ObterQtdPontosPerfilStatus>
    {
        public ObterQtdPontosPerfilOutput()
        {
            
        }

        public int Qtd { get; set; }

        public static ObterQtdPontosPerfilOutput QtdObtida(int qtd)
        {
            return new ObterQtdPontosPerfilOutput
            {
                Qtd = qtd,
                Status = ObterQtdPontosPerfilStatus.QtdObtida,
                Mensagem = "Quantidade obtida com sucesso."
            };
        }

        public static ObterQtdPontosPerfilOutput QtdNãoObtida(string mensagem = "")
        {
            return new ObterQtdPontosPerfilOutput
            {
                Status = ObterQtdPontosPerfilStatus.QtdNãoObtida,
                Mensagem = mensagem
            };
        }

    }
}
