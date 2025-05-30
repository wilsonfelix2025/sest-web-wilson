using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Sobrecarga.Factory
{
    public interface ICálculoSobrecargaFactory
    {
        ValidationResult CreateCálculoSobrecarga(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, DadosGerais dadosGerais, out ICálculoSobrecarga cálculo);
    }
}
