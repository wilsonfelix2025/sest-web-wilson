using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.GerarRelatórioUseCase
{
    public class GerarRelatórioOutput : UseCaseOutput<GerarRelatórioStatus>
    {
        public string Caminho { get; set; }
        public GerarRelatórioOutput()
        {
            
        }

        public static GerarRelatórioOutput Sucesso(string caminho)
        {
            return new GerarRelatórioOutput
            {
                Status = GerarRelatórioStatus.Sucesso,
                Mensagem = "",
                Caminho = caminho
            };
        }

        public static GerarRelatórioOutput Falha(string msg)
        {
            return new GerarRelatórioOutput
            {
                Status = GerarRelatórioStatus.Falha,
                Mensagem = $"{msg}"
            };
        }
    }
}
