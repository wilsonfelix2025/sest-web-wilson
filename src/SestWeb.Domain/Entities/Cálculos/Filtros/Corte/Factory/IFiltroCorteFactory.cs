using FluentValidation.Results;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Corte.Factory
{
    public interface IFiltroCorteFactory
    {
        ValidationResult CreateFiltroCorte(string nome, string grupoCálculo, PerfilBase perfilEntrada,
            PerfilBase perfilSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, out IFiltro filtro);
    }
}
