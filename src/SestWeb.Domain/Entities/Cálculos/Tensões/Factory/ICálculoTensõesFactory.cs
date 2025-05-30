using FluentValidation.Results;
using SestWeb.Domain.DTOs.Cálculo.TensõesInSitu;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.DadosGeraisDoPoco.GeometriaDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Enums;
using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.Tensões.Factory
{
    public interface ICálculoTensõesFactory
    {
        ValidationResult CreateCálculoTensões(string nome, MetodologiaCálculoTensãoHorizontalMenorEnum metodologiaCálculoTHORmin, string grupoCálculo,
            IList<PerfilBase> perfisEntrada, IList<PerfilBase> perfisSaída, Trajetória trajetória, ILitologia litologia, List<ParâmetrosLotDTO> parâmetrosLOT
            , DepleçãoDTO depleção, double? coeficiente, BreakoutDTO breakout, RelaçãoTensãoDTO relaçãoTensão, FraturasTrechosVerticaisDTO fraturasTrechosVerticais
            , Geometria geometria, DadosGerais dadosGerais, MetodologiaCálculoTensãoHorizontalMaiorEnum metodologiaCálculoTHORmax, out ICálculoTensões cálculo);
    }
}
