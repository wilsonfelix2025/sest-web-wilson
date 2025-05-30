using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados.EstratigrafiaInput;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados.GeometriaDoPoçoInput;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados.ObjetivoInput;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados.SapataInput;

namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados
{
    public class AtualizarDadosInput
    {
        public GeometriaInput Geometria { get; set; } 
        public IdentificaçãoInput Identificação { get; set; } 
        public AreaInput Area { get; set; } 
        public TipoProfundidadeSapata ProfundidadeReferênciaSapata { get; set; }
        public SapataChild[] Sapatas { get; set; }
        public TipoProfundidadeObjetivo ProfundidadeReferênciaObjetivo { get; set; }
        public ObjetivoChild[] Objetivos { get; set; }
        public TipoProfundidadeEstratigrafia ProfundidadeReferênciaEstratigrafia { get; set; }
        public EstratigrafiaChild[] Estratigrafias { get; set; }
        public double PmFinal { get; set; } = 0;
        public bool ÉVertical { get; set; } = false;
    }
}
