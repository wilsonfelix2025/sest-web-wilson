using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Trend;

namespace SestWeb.Application.UseCases.TrendUseCases.CriarTrend
{
    public class CriarTrendOutput : UseCaseOutput<CriarTrendStatus>
    {
        public CriarTrendOutput()
        {
            
        }

        public Trend Trend { get; set; }
        public static CriarTrendOutput TrendCriado(Trend trend)
        {
            return new CriarTrendOutput
            {
                Status = CriarTrendStatus.TrendCriado,
                Mensagem = "Trend criado com sucesso",
                Trend = trend
            };
        }

        public static CriarTrendOutput TrendNãoCriado(string msg)
        {
            return new CriarTrendOutput
            {
                Status = CriarTrendStatus.TrendNãoCriado,
                Mensagem = $"Não foi possível criar trend. {msg}"
            };
        }
    }
}
