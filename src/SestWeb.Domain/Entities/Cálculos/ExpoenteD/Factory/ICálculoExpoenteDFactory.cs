using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.ExpoenteD.Factory
{
    public interface ICálculoExpoenteDFactory
    {
        ValidationResult CreateCálculoExpoenteD(string nome, string grupoCálculo,
            IList<PerfilBase> ExpoenteDEntrada, IList<PerfilBase> ExpoenteDSaída, Trajetória trajetória
            , ILitologia litologia, string correlação, out ICálculoExpoenteD cálculo);
    }
}
