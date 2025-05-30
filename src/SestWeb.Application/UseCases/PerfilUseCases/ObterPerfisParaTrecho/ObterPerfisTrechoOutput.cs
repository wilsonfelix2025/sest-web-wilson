using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisParaTrecho
{
    public class ObterPerfisTrechoOutput : UseCaseOutput<ObterPerfisTrechoStatus>
    {
        public ObterPerfisTrechoOutput()
        {
            
        }

        public ObterPerfisTrechoModel ObterPerfisTrechoModel { get; set; }

        public static ObterPerfisTrechoOutput PerfisObtidos(ObterPerfisTrechoModel dados)
        {
            return new ObterPerfisTrechoOutput
            {
                ObterPerfisTrechoModel = dados,
                Status = ObterPerfisTrechoStatus.PerfisObtidos,
                Mensagem = "Perfis obtidos com sucesso."
            };
        }

        public static ObterPerfisTrechoOutput PerfisNãoObtidos(string mensagem = "")
        {
            return new ObterPerfisTrechoOutput
            {
                Status = ObterPerfisTrechoStatus.PerfisNãoObtidos,
                Mensagem = mensagem
            };
        }

    }
}
