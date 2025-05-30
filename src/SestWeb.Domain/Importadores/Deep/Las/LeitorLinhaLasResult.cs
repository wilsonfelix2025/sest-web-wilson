namespace SestWeb.Domain.Importadores.Deep.Las
{
    public class LeitorLinhaLasResult
    {
        public readonly LeitorLinhaLasResultType Tipo;
        public readonly string Seção;

        public LeitorLinhaLasResult(LeitorLinhaLasResultType tipoResultado, string seçãoDaLinha)
        {
            Tipo = tipoResultado;
            Seção = seçãoDaLinha;
        }
    }    
}
