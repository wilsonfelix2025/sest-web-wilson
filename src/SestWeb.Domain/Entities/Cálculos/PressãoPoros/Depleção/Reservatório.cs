using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Depleção
{
    public class Reservatório
    {
        public string Nome { get; set; }
        public Profundidade ProfundidadeTopo { get; set; }
        public Profundidade ProfundidadeBase { get; set; }
        public double ValorGásÁgua { get; set; }
        public double ValorGásÓleo { get; set; }
        public double ValorGásGás { get; set; }
        public Profundidade ContatoGásÓleo { get; set; }
        public Profundidade ContatoÓleoÁgua { get; set; }
    }
}
