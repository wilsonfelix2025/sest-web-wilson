namespace SestWeb.Application.UseCases.EstratigrafiaUseCases.AtualizarEstratigrafias
{
    public class AtualizarEstratigrafiasInput
    {
        public AtualizarEstratigrafiasInput(TipoProfundidade profundidadeReferência, EstratigrafiaChild[] estratigrafias)
        {
            ProfundidadeReferência = profundidadeReferência;
            Estratigrafias = estratigrafias;
        }

        public TipoProfundidade ProfundidadeReferência { get; }
        public EstratigrafiaChild[] Estratigrafias { get; }
    }
}
