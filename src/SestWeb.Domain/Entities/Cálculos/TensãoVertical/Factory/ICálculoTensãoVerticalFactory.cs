using FluentValidation.Results;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.TensãoVertical.Factory
{
    public interface ICálculoTensãoVerticalFactory
    {
        ValidationResult CreateCálculoTensãoVertical(string nome, string grupoCálculo, IList<PerfilBase> perfisEntrada,
            IList<PerfilBase> perfisSaída, Trajetória trajetóriaOld, ILitologia litologia, Geometria geometria, DadosGerais dadosGerais, out ICálculoTensãoVertical cálculo);
    }
}
    