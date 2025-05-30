
namespace SestWeb.Application.UseCases.SapataUseCases.AtualizarSapatas
{
    public class AtualizarSapatasInput
    {
        public AtualizarSapatasInput(TipoProfundidade profundidadeReferência, SapataChild[] sapatas)
        {
            ProfundidadeReferência = profundidadeReferência;
            Sapatas = sapatas;
        }

        public TipoProfundidade ProfundidadeReferência { get; }
        public SapataChild[] Sapatas { get; }
    }
}
