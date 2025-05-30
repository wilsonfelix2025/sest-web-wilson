using System.ComponentModel;

namespace SestWeb.Domain.Entities.Correlações.OrigemCorrelação
{
    public enum Origem
    {
        [Description("Correlação fixa do Sest")]
        Fixa,
        [Description("Correlação criada pelo usuário")]
        Usuário,
        [Description("Correlação criada pelo usuário exclusiva do Caso")]
        Poço,
        [Description("Correlação composta no cálculo de perfis")]
        CálculoDePerfis,
        [Description("Correlação composta no cálculo de Sobrecarga")]
        CálculoDeSobrecarga,
        [Description("Correlação composta no cálculo de Propriedades Mecânicas")]
        CálculoDePropriedadesMecânicas,
        [Description("Correlação composta no cálculo de Gradientes")]
        CálculoDeGradientes,
        [Description("Correlação composta no cálculo de Pressão de Poros")]
        CálculoDePressãoDePoros,
        [Description("Correlação composta no cálculo de Tensões In Situ")]
        Tensões
    }
}
