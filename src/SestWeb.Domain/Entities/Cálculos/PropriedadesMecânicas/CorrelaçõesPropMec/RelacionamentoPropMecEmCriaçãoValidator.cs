using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.CorrelaçõesPropMec
{
    public class RelacionamentoPropMecEmCriaçãoValidator : AbstractValidator<RelacionamentoPropMecEmCriação>, IRelacionamentoPropMecEmCriaçãoValidator
    {
        public RelacionamentoPropMecEmCriaçãoValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(rel => rel.GrupoLitológico).NotNull().When(relacionamento => relacionamento != null)
                .WithMessage("Grupo litológico do relacionamento não pode ser null!");

            RuleFor(rel => rel.GrupoLitológico)
                .Must(grupoLito => GrupoLitológicoDeveSerVálido(grupoLito))
                .When(relacionamento => relacionamento != null)
                .WithMessage(
                    $"GrupoLitológico do relacionamento deve ser uma das seguintes opções: {"\n"}{string.Join("\n", GrupoLitologico.GetNames())}!");

            RuleFor(rel => rel.NomeAutor).NotNull().When(relacionamento => relacionamento != null)
                .WithMessage("Nome do autor não pode ser null!");

            RuleFor(rel => rel.ChaveAutor).NotNull().When(relacionamento => relacionamento != null)
                .WithMessage("Chave do autor não pode ser null!");

            RuleFor(rel => rel.CorrUcs).NotNull().When(relacionamento => relacionamento != null)
                .WithMessage("Correlação de ucs não pode ser null!");

            RuleFor(rel => rel.CorrCoesa).NotNull().When(relacionamento => relacionamento != null)
                .WithMessage("Correlação de coesa não pode ser null!");

            RuleFor(rel => rel.CorrAngat).NotNull().When(relacionamento => relacionamento != null)
                .WithMessage("Correlação de angat não pode ser null!");

            RuleFor(rel => rel.CorrRestr).NotNull().When(relacionamento => relacionamento != null)
                .WithMessage("Correlação de restr não pode ser null!");

            RuleFor(rel => rel.Origem).NotNull().When(relacionamento => relacionamento != null)
                .WithMessage("Origem do relacionamento não pode ser null!");

            RuleFor(rel => rel.Origem)
                .Must(origem => OrigemDeveSerVálida(origem))
                .When(relacionamento => relacionamento != null)
                .WithMessage(
                    $"Origem do relacionamento deve ser uma das seguintes opções: {"\n"}{string.Join("\n", Enum.GetNames(typeof(Origem)))}!");//string.Join("\n", result.Errors)
        }

        protected override bool PreValidate(ValidationContext<RelacionamentoPropMecEmCriação> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(typeof(RelacionamentoPropMecEmCriação).Name,
                    "Relacionamento não pode ser null!"));
                return false;
            }

            return true;
        }

        private bool OrigemDeveSerVálida(string origem)
        {
            return Enum.TryParse(origem, out Origem value);
        }

        private bool GrupoLitológicoDeveSerVálido(string grupoLito)
        {
            var gruposLitológicos = GrupoLitologico.GetNames();
            return gruposLitológicos.Any(gl => string.Equals(gl, grupoLito.Trim()));
        }
    }
}
