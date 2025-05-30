using System;
using FluentValidation;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Entities.Cálculos.Tensões.EmCriação
{
    /// <summary>
    /// Validar aqui propriedades que permitam criar CálculoTensões em segurança.
    /// </summary>
    public class CálculoTensõesEmCriaçãoValidator : CálculoEmCriaçãoValidator<CálculoTensõesEmCriação>, ICálculoTensõesEmCriaçãoValidator
    {
        public CálculoTensõesEmCriaçãoValidator()
        {
            // exemplo: (já está implementado no validador base.)
            RuleFor(calc => calc.GrupoCálculo)
                .Must(grupoCálculo => GrupoCálculoDeveSerVálido(grupoCálculo))
                .When(correlação => correlação != null)
                .WithMessage(
                    $"GrupoCálculo da cálculo deve ser uma das seguintes opções: {"\n"}{string.Join("\n", Enum.GetNames(typeof(GrupoCálculo)))}!");

            RuleFor(calc => calc.Litologia)
                .Must(lito => LitologiaDeveConterTrechos(lito))                
                .WithMessage(
                    $"Litologia padrão do poço deve conter trechos");

            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
        }

        private bool GrupoCálculoDeveSerVálido(string grupoCálculo)
        {
            return Enum.TryParse(grupoCálculo, out GrupoCálculo value);
        }

        private bool LitologiaDeveConterTrechos(ILitologia litologia)
        {
            if (litologia.Pontos.Count == 0)
                return false;

            return true;
        }
    }
}
