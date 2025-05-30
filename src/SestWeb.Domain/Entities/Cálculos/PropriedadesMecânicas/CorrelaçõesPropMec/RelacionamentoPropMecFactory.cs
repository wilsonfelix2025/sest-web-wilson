using System;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec.EditingRelacionamentoPropMec;
using SestWeb.Domain.Entities.Correlações.AutorCorrelação;
using SestWeb.Domain.Entities.Correlações.Base;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{

    public class RelacionamentoPropMecFactory : IRelacionamentoPropMecFactory
    {
        private readonly IRelacionamentoPropMecEmCriaçãoValidator _relacionamentoPropMecEmCriaçãoValidator;
        private readonly IRelacionamentoPropMecEmEdiçãoValidator _relacionamentoEmEdiçãoValidator;
        private readonly IRelacionamentoPropMecValidator _relacionamentoPropMecValidator;
        private static Func<GrupoLitologico, Origem, IAutor, Correlação, Correlação, Correlação, Correlação, RelacionamentoUcsCoesaAngatPorGrupoLitológico> _ctorCaller;

        public RelacionamentoPropMecFactory(IRelacionamentoPropMecEmCriaçãoValidator relacionamentoPropMecEmCriaçãoValidator, IRelacionamentoPropMecEmEdiçãoValidator relacionamentoEmEdiçãoValidator, IRelacionamentoPropMecValidator relacionamentoPropMecValidator)
        {
            _relacionamentoPropMecEmCriaçãoValidator = relacionamentoPropMecEmCriaçãoValidator;
            _relacionamentoEmEdiçãoValidator = relacionamentoEmEdiçãoValidator;
            _relacionamentoPropMecValidator = relacionamentoPropMecValidator;
        }

        public static void RegisterCorrelaçãoCtor(Func<GrupoLitologico, Origem, IAutor, Correlação, Correlação, Correlação, Correlação, RelacionamentoUcsCoesaAngatPorGrupoLitológico> ctorCaller)
        {
            _ctorCaller = ctorCaller;
        }

        public ValidationResult CreateRelacionamentoPropMec(string grupoLitológico, string origem, string nomeAutor, string chaveAutor, Correlação corrUcs, Correlação corrCoesa, Correlação corrAngat, Correlação corrRestr, out RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamentoUcsCoesaAngatPorGrupoLitológico)
        {
            // Verifica se a correlação pode ser montada em segurança, mesmo que com dados inválidos.
            var result =
                EnsureRelacionamentoCreation(grupoLitológico, origem, nomeAutor, chaveAutor, corrUcs, corrCoesa, corrAngat, corrRestr);

            if (!result.IsValid)
            {
                relacionamentoUcsCoesaAngatPorGrupoLitológico = default;
                return result;
            }

            // Monta as dependências
            Origem origin = (Origem)Enum.Parse(typeof(Origem), origem, true);
            var autor = Autor.GetFactory().CreateAutor(nomeAutor, chaveAutor, DateTime.Now);
            var grupoLito = GrupoLitologico.GetFromName(grupoLitológico);

            // Solicita o registro do construtor
            RelacionamentoUcsCoesaAngatPorGrupoLitológico.RegisterRelacionamentoPropMecCtor();

            // Chama o construtor
            relacionamentoUcsCoesaAngatPorGrupoLitológico = _ctorCaller(grupoLito, origin, autor, corrUcs, corrCoesa, corrAngat, corrRestr);

            // Valida a entidade 
            return _relacionamentoPropMecValidator.Validate(relacionamentoUcsCoesaAngatPorGrupoLitológico);
        }

        private ValidationResult EnsureRelacionamentoCreation(string grupoLitológico, string origem, string nomeAutor, string chaveAutor, Correlação corrUcs, Correlação corrCoesa, Correlação corrAngat, Correlação corrRestr)
        {
            RelacionamentoPropMecEmCriação relacionamentoPropMecEmCriação =
                new RelacionamentoPropMecEmCriação(grupoLitológico, origem, nomeAutor, chaveAutor, corrUcs, corrCoesa, corrAngat, corrRestr);

            return _relacionamentoPropMecEmCriaçãoValidator.Validate(relacionamentoPropMecEmCriação);
        }

        public ValidationResult UpdateRelacionamentoPropMec(GrupoLitologico grupoLitológico, string origem, string nomeAutor, string chaveAutor, Correlação corrUcs, Correlação corrCoesa, Correlação corrAngat, Correlação corrRestr, out RelacionamentoUcsCoesaAngatPorGrupoLitológico relacionamentoUcsCoesaAngatPorGrupoLitológico)
        {
            var result = EnsureCorrelaçãoUpdate(origem);
            if (!result.IsValid)
            {
                relacionamentoUcsCoesaAngatPorGrupoLitológico = default;
                return result;
            }

            return CreateRelacionamentoPropMec(grupoLitológico.Nome, origem, nomeAutor, chaveAutor, corrUcs, corrCoesa,
                corrAngat, corrRestr,
                out  relacionamentoUcsCoesaAngatPorGrupoLitológico);
        }

        private ValidationResult EnsureCorrelaçãoUpdate(string origem)
        {
            RelacionamentoPropMecEmEdição relacionamentoEmEdição = new RelacionamentoPropMecEmEdição(origem);
            return _relacionamentoEmEdiçãoValidator.Validate(relacionamentoEmEdição);
        }
    }
}
