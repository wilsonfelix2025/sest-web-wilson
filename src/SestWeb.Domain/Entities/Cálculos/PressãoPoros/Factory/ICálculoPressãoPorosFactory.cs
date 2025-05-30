using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Base;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Reservatório;
using SestWeb.Domain.Entities.Correlações.ParâmetroCorrelação;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.PressãoPoros.Factory
{
    public interface ICálculoPressãoPorosFactory
    {
        ValidationResult CreateCálculoPressãoPoros(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, CorrelaçãoPressãoPoros métodoCálculo, IList<ParâmetroCorrelação> parâmetrosCorrelação,
            Trajetória trajetória, ILitologia litologia, DadosGerais dadosGerais, DadosReservatório reservatório, out ICálculoPressãoPoros cálculo);
    }
}
