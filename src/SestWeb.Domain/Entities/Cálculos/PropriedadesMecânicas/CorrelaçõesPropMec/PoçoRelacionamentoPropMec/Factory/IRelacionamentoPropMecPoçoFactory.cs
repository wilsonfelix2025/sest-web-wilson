using FluentValidation.Results;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.PoçoRelacionamentoPropMec.Factory
{
    public interface IRelacionamentoPropMecPoçoFactory
    {
        ValidationResult CreateRelacionamentoPropMecPoço(string idPoço, string grupoLitológico,
            string nomeAutor, string chaveAutor, Correlação corrUcs, Correlação corrCoesa, Correlação corrAngat,
            Correlação corrRestr, out RelacionamentoPropMecPoço relacionamentoPropMecPoço);

        ValidationResult UpdateRelacionamentoPropMecPoço(string idPoço, string nome, GrupoLitologico grupoLitológico, string origem,
            string nomeAutor, string chaveAutor, Correlação corrUcs, Correlação corrCoesa, Correlação corrAngat,
            Correlação corrRestr, out IRelacionamentoPropMecPoço relacionamentoPropMecPoço);
    }
}
