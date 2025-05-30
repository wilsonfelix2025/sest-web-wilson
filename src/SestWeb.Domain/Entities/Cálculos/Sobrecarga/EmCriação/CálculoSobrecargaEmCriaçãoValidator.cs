using System;
using FluentValidation;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;

namespace SestWeb.Domain.Entities.Cálculos.Sobrecarga.EmCriação
{
    /// <summary>
    /// Validar aqui propriedades que permitam criar CálculoPressãoPoros em segurança.
    /// </summary>
    public class CálculoSobrecargaEmCriaçãoValidator : CálculoEmCriaçãoValidator<CálculoSobrecargaEmCriação>, ICálculoSobrecargaEmCriaçãoValidator
    {
        public CálculoSobrecargaEmCriaçãoValidator()
        {
            // exemplo: (já está implementado no validador base.)
            RuleFor(calcPerfis => calcPerfis.GrupoCálculo)
                .Must(grupoCálculo => GrupoCálculoDeveSerVálido(grupoCálculo))
                .When(correlação => correlação != null)
                .WithMessage(
                    $"GrupoCálculo da cálculo deve ser uma das seguintes opções: {"\n"}{string.Join("\n", Enum.GetNames(typeof(GrupoCálculo)))}!");

            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
        }

        private bool GrupoCálculoDeveSerVálido(string grupoCálculo)
        {
            return Enum.TryParse(grupoCálculo, out GrupoCálculo value);
        }
    }
}
