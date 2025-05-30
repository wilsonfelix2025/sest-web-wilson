using System;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.PoçoRelacionamentoPropMec.Factory
{
    public class RelacionamentoPropMecPoçoFactory : IRelacionamentoPropMecPoçoFactory
    {
        private readonly IRelacionamentoPropMecFactory _relacionamentoPropMecFactory;
        private static Func<string, RelacionamentoUcsCoesaAngatPorGrupoLitológico, RelacionamentoPropMecPoço> _ctorCaller;

        public RelacionamentoPropMecPoçoFactory(IRelacionamentoPropMecFactory relacionamentoPropMecFactory)
        {
            _relacionamentoPropMecFactory = relacionamentoPropMecFactory;
        }

        public ValidationResult CreateRelacionamentoPropMecPoço(string idPoço, string grupoLitológico, string nomeAutor, string chaveAutor, Correlação corrUcs, Correlação corrCoesa, Correlação corrAngat, Correlação corrRestr, out RelacionamentoPropMecPoço relacionamentoPropMecPoço)
        {
            var result = _relacionamentoPropMecFactory
                .CreateRelacionamentoPropMec(grupoLitológico, Origem.Poço.ToString(), nomeAutor,chaveAutor,corrUcs,corrCoesa,corrAngat,corrRestr, out RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamentoPropMec);

            if (!result.IsValid)
            {
                relacionamentoPropMecPoço = default;
                return result;
            }

            RelacionamentoPropMecPoço.RegisterRelacionamentoPropMecPoçoCtor();
            relacionamentoPropMecPoço = _ctorCaller(idPoço, relacionamentoPropMec);
            return result;
        }

        public ValidationResult UpdateRelacionamentoPropMecPoço(string idPoço, string nome, GrupoLitologico grupoLitológico, string origem, string nomeAutor, string chaveAutor, Correlação corrUcs, Correlação corrCoesa, Correlação corrAngat, Correlação corrRestr, out IRelacionamentoPropMecPoço relacionamentoPropMecPoço)
        {
            var result = _relacionamentoPropMecFactory.UpdateRelacionamentoPropMec(grupoLitológico, origem, nomeAutor,
                chaveAutor, corrUcs, corrCoesa, corrAngat, corrRestr,
                out RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamentoPropMec);

            if (!result.IsValid)
            {
                relacionamentoPropMecPoço = default;
                return result;
            }

            RelacionamentoPropMecPoço.RegisterRelacionamentoPropMecPoçoCtor();
            relacionamentoPropMecPoço = _ctorCaller(idPoço, relacionamentoPropMec);
            return result;
        }

        public static void RegisterRelacionamentoPropMecPoçoCtor(Func<string, RelacionamentoUcsCoesaAngatPorGrupoLitológico, RelacionamentoPropMecPoço> ctorCaller)
        {
            _ctorCaller = ctorCaller;
        }
    }
}
