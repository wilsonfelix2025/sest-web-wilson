using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Trend;

namespace SestWeb.Application.UseCases.TrendUseCases.ObterTrend
{
    public class ObterTrendOutput : UseCaseOutput<ObterTrendStatus>
    {
        public ObterTrendOutput()
        {
            
        }

        public Trend Trend { get; set; }
        public static ObterTrendOutput TrendObtido(Trend trend)
        {
            return new ObterTrendOutput
            {
                Status = ObterTrendStatus.TrendObtido,
                Mensagem = "Trend obtido com sucesso",
                Trend = trend
            };
        }

        public static ObterTrendOutput TrendNãoObtido(string msg)
        {
            return new ObterTrendOutput
            {
                Status = ObterTrendStatus.TrendNãoObtido,
                Mensagem = $"Não foi possível obter trend. {msg}"
            };
        }
    }
}
