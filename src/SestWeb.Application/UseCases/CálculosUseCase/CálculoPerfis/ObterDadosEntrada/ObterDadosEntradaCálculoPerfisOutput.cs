using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.ObterDadosEntrada
{
    public class ObterDadosEntradaCálculoPerfisOutput : UseCaseOutput<ObterDadosEntradaCálculoPerfisStatus>
    {
        public ObterDadosEntradaCálculoPerfisOutput()
        {
            
        }

        public ICálculo Cálculo { get; set; }

        public static ObterDadosEntradaCálculoPerfisOutput DadosObtidos(ICálculo cálculo)
        {
            return new ObterDadosEntradaCálculoPerfisOutput
            {
                Status = ObterDadosEntradaCálculoPerfisStatus.DadosObtidos,
                Mensagem = "Dados obtido com sucesso",
                Cálculo = cálculo
            };
        }

        public static ObterDadosEntradaCálculoPerfisOutput DadosNãoObtidos(string msg)
        {
            return new ObterDadosEntradaCálculoPerfisOutput
            {
                Status = ObterDadosEntradaCálculoPerfisStatus.DadosNãoObtidos,
                Mensagem = $"Não foi possível obter dados de entrada do cálculo. {msg}"
            };
        }
    }
}
