namespace SestWeb.Domain.Entities.Cálculos.Gradientes.Tensões
{
    public interface ITensoesAoRedorPoco
    {
        double TensaoRadial { get; }
        double TensaoTangencial { get; }
        double TensaoAxial { get; }
        double TensaoCisalhanteNoPlanoRadialTangencial { get; }
        double TensaoCisalhanteNoPlanoTangencialAxial { get; }
        double TensaoCisalhanteNoPlanoRadialAxial { get; }

        double Pressao { get; }
    }
}
