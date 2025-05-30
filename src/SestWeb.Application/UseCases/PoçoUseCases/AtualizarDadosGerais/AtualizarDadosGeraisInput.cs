using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDadosGerais.GeometriaDoPoçoInput;

namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarDadosGerais
{
    public class AtualizarDadosGeraisInput
    {
        public GeometriaInput Geometria { get; set; } = new GeometriaInput();
        public IdentificaçãoInput Identificação { get; set; } = new IdentificaçãoInput();
        public AreaInput Area { get; set; } = new AreaInput();
    }
}
