using SestWeb.Domain.DTOs.Cálculo.Gradientes;
using SestWeb.Domain.Entities.Cálculos.Base;
using SestWeb.Domain.Entities.Cálculos.Gradientes.Malha;

namespace SestWeb.Domain.Entities.Cálculos.Gradientes
{
    public interface ICálculoGradientes : ICálculo{

        DadosMalha Dadosmalha { get;  }
        EntradasColapsosDTO EntradaColapsos { get; }


    }
}
