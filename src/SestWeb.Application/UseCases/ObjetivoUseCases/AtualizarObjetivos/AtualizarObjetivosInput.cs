
namespace SestWeb.Application.UseCases.ObjetivoUseCases.AtualizarObjetivos
{
    public class AtualizarObjetivosInput
    {
        public AtualizarObjetivosInput(TipoProfundidade profundidadeReferência, ObjetivoChild[] objetivos)
        {
            ProfundidadeReferência = profundidadeReferência;
            Objetivos = objetivos;
        }

        public TipoProfundidade ProfundidadeReferência { get; }
        public ObjetivoChild[] Objetivos { get; }
    }
}
