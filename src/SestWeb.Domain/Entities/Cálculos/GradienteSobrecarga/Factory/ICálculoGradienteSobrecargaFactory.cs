using FluentValidation.Results;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.GradienteSobrecarga.Factory
{
    public interface ICálculoGradienteSobrecargaFactory
    {
        ValidationResult CreateCálculoGradienteSobrecarga(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, out ICálculoGradienteSobrecarga cálculo);
    }
}
