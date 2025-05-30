using SestWeb.Application.Helpers;


namespace SestWeb.Application.UseCases.TrendUseCases.RemoverTrend
{
    public class RemoverTrendOutput : UseCaseOutput<RemoverTrendStatus>
    {
        public RemoverTrendOutput()
        {
            
        }

        public static RemoverTrendOutput TrendRemovido()
        {
            return new RemoverTrendOutput
            {
                Status = RemoverTrendStatus.TrendRemovido,
                Mensagem = "Trend removido com sucesso",
            };
        }

        public static RemoverTrendOutput TrendNãoRemovido(string msg)
        {
            return new RemoverTrendOutput
            {
                Status = RemoverTrendStatus.TrendNãoRemovido,
                Mensagem = $"Não foi possível remover trend. {msg}"
            };
        }
    }
}
