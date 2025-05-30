using System.Collections.Generic;
using FluentValidation.Results;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Litologia.Factory
{
    public interface IFiltroLitologiaFactory
    {
        ValidationResult CreateFiltroLitologia(string nome, string grupoCálculo, PerfilBase perfilEntrada,
            PerfilBase perfilSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior,
            double? limiteSuperior, TipoCorteEnum? tipoCorte, List<string> litologias, out IFiltro filtro);
    }
}
