using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;

namespace SestWeb.Domain.Entities.DadosGeraisDoPoco
{
    public class DadosGerais
    {
        public Geometria Geometria { get; set; } = new Geometria();
        public Identificação Identificação { get; set; } = new Identificação();
        public Area Area { get; set; } = new Area();
    }
}