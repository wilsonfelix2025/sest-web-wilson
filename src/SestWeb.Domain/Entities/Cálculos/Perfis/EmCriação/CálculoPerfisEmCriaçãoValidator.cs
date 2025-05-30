using System;
using FluentValidation;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;

namespace SestWeb.Domain.Entities.Cálculos.Perfis.EmCriação
{
    /// <summary>
    /// Validar aqui propriedades que permitam criar CálculoPerfis em segurança.
    /// </summary>
    public class CálculoPerfisEmCriaçãoValidator : CálculoEmCriaçãoValidator<CálculoPerfisEmCriação>, ICálculoPerfisEmCriaçãoValidator
    {
        public CálculoPerfisEmCriaçãoValidator()
        {
            // exemplo: (já está implementado no validador base.)
            RuleFor(calcPerfis => calcPerfis.GrupoCálculo)
                .Must(GrupoCálculoDeveSerVálido)
                .When(correlação => correlação != null)
                .WithMessage(
                    $"GrupoCálculo da cálculo deve ser uma das seguintes opções: \n{string.Join("\n", Enum.GetNames(typeof(GrupoCálculo)))}!");

            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(c => c.ListaCorrelação).NotNull().NotEmpty();
            RuleFor(c => c.DadosGerais).NotNull();
            RuleFor(c => c.Geometria).NotNull();
            RuleFor(c => c.Litologia).NotNull().NotEmpty();

        }

        private bool GrupoCálculoDeveSerVálido(string grupoCálculo)
        {
            return Enum.TryParse(grupoCálculo, out GrupoCálculo value);
        }
    }
}
