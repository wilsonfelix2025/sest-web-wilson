namespace SestWeb.Domain.Entities.DadosGeraisDoPoco
{
    public enum TipoPoço
    {
        Projeto = 0,
        Retroanalise = 1,
        Monitoramento = 2,
    }
    public class Tipo
    {
        public static TipoPoço GetTipo(string tipo)
        {
            if (tipo == "sesttr.project")
            {
                return TipoPoço.Projeto;
            }
            else if (tipo == "sesttr.monitoring")
            {
                return TipoPoço.Monitoramento;
            }
            else
            {
                return TipoPoço.Retroanalise;
            }
        }
    }
}