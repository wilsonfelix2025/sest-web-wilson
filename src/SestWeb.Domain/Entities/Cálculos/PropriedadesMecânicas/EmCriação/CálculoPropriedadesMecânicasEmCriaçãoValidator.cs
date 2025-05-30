using FluentValidation;
using SestWeb.Domain.Entities.Cálculos.Base.EmCriação;
using SestWeb.Domain.Entities.Cálculos.Base.GrupoDeCálculo;
using System;
using System.Collections.Generic;

namespace SestWeb.Domain.Entities.Cálculos.PropriedadesMecânicas.EmCriação
{
    /// <summary>
    /// Validar aqui propriedades que permitam criar CálculoPropriedadesMecânica em segurança.
    /// </summary>
    public class CálculoPropriedadesMecânicasEmCriaçãoValidator : CálculoEmCriaçãoValidator<CálculoPropriedadesMecânicasEmCriação>, ICálculoPropriedadesMecânicasEmCriaçãoValidator
    {
        public CálculoPropriedadesMecânicasEmCriaçãoValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(calc => calc.GrupoCálculo)
                .Must(GrupoCálculoDeveSerVálido)
                .When(correlação => correlação != null)
                .WithMessage(
                    $"GrupoCálculo da cálculo deve ser uma das seguintes opções: \n{string.Join("\n", Enum.GetNames(typeof(GrupoCálculo)))}!");

            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            When(c => c.CorrelaçãoDoCálculo.Length == 0, () =>
           {
               RuleFor(c => c.ListaCorrelação).NotNull().NotEmpty();
               RuleFor(calc => calc.Regiões).Custom((x, context) =>
               {
                   var validação = SóPodeUmaCorrelaçãoCalculada(x);
                   if (!string.IsNullOrWhiteSpace(validação))
                   {
                       context.AddFailure(validação);
                   }
               });
           });

            RuleFor(c => c.DadosGerais).NotNull();
            RuleFor(c => c.Geometria).NotNull();
            RuleFor(c => c.Litologia).NotNull().NotEmpty();

        }

        private bool GrupoCálculoDeveSerVálido(string grupoCálculo)
        {
            return Enum.TryParse(grupoCálculo, out GrupoCálculo value);
        }

        private string SóPodeUmaCorrelaçãoCalculada(IList<CorrelaçõesDefaultPorGrupoLitológicoCálculoPropriedadesMecânicas> regiões)
        {
            var retorno = string.Empty;
            
            foreach (var região in regiões)
            {
                var angat = Convert.ToInt32(região.Angat.Nome.Contains("CALCULADO"));
                var coesa = Convert.ToInt32(região.Coesa.Nome.Contains("CALCULADO"));
                var restr = Convert.ToInt32(região.Restr.Nome.Contains("CALCULADO"));
                var ucs = Convert.ToInt32(região.Ucs.Nome.Contains("CALCULADO"));

                if (angat + coesa + restr + ucs > 1)
                {
                    retorno = "Há mais de uma correlaçõe com o nome CALCULADO para o grupo litológico" ;
                    return retorno;
                }
            }

            return retorno;
        }
    }
}
