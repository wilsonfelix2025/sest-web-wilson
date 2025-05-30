using System;
using FluentValidation;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;

namespace SestWeb.Domain.Entities.Cálculos.Base.EmCriação
{
    public class CálculoEmCriaçãoValidator<T> : AbstractValidator<T>, ICálculoEmCriaçãoValidator<T> where T : CálculoEmCriação
    {
        protected CálculoEmCriaçãoValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(calc => calc.Nome).NotNull().When(cálculo => cálculo != null)
                .WithMessage("Nome do cálculo não pode ser null!");

            RuleFor(calc => calc.GrupoCálculo).NotNull().When(cálculo => cálculo != null)
                .WithMessage("GrupoCálculo não pode ser null!");

            RuleFor(calc => calc.PerfisEntrada).NotNull().When(cálculo => cálculo != null)
                .WithMessage("PerfisEntrada não pode ser null!");

            RuleFor(calc => calc.PerfisSaída).NotNull().When(cálculo => cálculo != null)
                .WithMessage("PerfisSaída não pode ser null!");

            //RuleFor(calc => calc.Trajetória).NotNull().When(cálculo => cálculo != null)
            //    .WithMessage("Trajetória não pode ser null!");

            //RuleFor(calc => calc.Litologia).NotNull().When(cálculo => cálculo != null)
            //    .WithMessage("Litologia não pode ser null!");

            RuleForEach(calc => calc.PerfisEntrada)
                .NotNull().WithMessage("Perfil de entrada, index: {CollectionIndex}, não pode ser null.");

            RuleForEach(calc => calc.PerfisSaída)
                .NotNull().WithMessage("Perfil de saída, index: {CollectionIndex}, não pode ser null.");

            RuleFor(calc => calc.GrupoCálculo)
                .Must(grupoCálculo => GrupoCálculoDeveSerVálido(grupoCálculo))
                .When(correlação => correlação != null)
                .WithMessage(
                    $"GrupoCálculo da cálculo deve ser uma das seguintes opções: {"\n"}{string.Join("\n", Enum.GetNames(typeof(GrupoCálculo)))}!");
        }

        private bool GrupoCálculoDeveSerVálido(string grupoCálculo)
        {
            return Enum.TryParse(grupoCálculo, out GrupoCálculo value);
        }
    }
}
