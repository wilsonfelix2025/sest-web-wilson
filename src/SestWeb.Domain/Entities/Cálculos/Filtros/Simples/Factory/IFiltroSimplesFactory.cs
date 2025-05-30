using FluentValidation.Results;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Entities.Cálculos.Filtros.Simples.Factory
{
    public interface IFiltroSimplesFactory
    {
        ValidationResult CreateFiltroSimples(string nome, string grupoCálculo, PerfilBase perfilEntrada,
            PerfilBase perfilSaída, Trajetória trajetória, ILitologia litologia, double? limiteInferior, double? limiteSuperior, TipoCorteEnum? tipoCorte, double desvioMáximo, out IFiltro filtro);
    }
}
