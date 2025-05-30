using System.Collections.Generic;
using FluentValidation;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.MontagemPerfis;

namespace SestWeb.Domain.Validadores
{
    public class MontarPerfisValidator : AbstractValidator<MontadorDePerfil>
    {
        public MontarPerfisValidator()
        {
            RuleFor(montador => montador.PoçoCorrelação).NotEmpty().WithMessage("Poço de correlação não encontrado");

            RuleFor(montador => montador.PoçoCorrelação.TipoPoço).Equal(TipoPoço.Retroanalise);

            RuleFor(montador => montador.PoçoCorrelação.Trajetória.Pontos).NotEmpty();

            RuleFor(montador => montador.Perfis).NotEmpty();

            RuleFor(montador => montador.Perfis).Custom((x, context) =>
            {
                var validação = ValidateTipoPerfil(x);
                if (!string.IsNullOrWhiteSpace(validação))
                {
                    context.AddFailure(validação);
                }
            });

            RuleFor(montador => montador.TopoCorrelação >= montador.PoçoCorrelação.ObterBaseDeSedimentos());

            RuleFor(montador => montador.BaseCorrelação <= montador.PoçoCorrelação.Trajetória.PvFinal.Valor);

            RuleFor(montador => montador.PoçoTrabalho.TipoPoço).Equal(TipoPoço.Projeto);

            RuleFor(montador => montador.PoçoTrabalho.Trajetória.Pontos).NotEmpty();

            RuleFor(montador => montador.TopoTrabalho >= montador.PoçoTrabalho.ObterBaseDeSedimentos());

            RuleFor(montador => montador.BaseTrabalho <= montador.PoçoTrabalho.Trajetória.PvFinal.Valor);

        }

        public string ValidateTipoPerfil(IList<PerfilBase> perfis)
        {
            var result = string.Empty;
            var tiposPerfil = new List<string>
            {
                "RHOB",
                "DTS",
                "DTC",
                "GRAY",
                "RESIST",
                "PORO"
            };

            foreach (var perfil in perfis)
            {
                if (!tiposPerfil.Contains(perfil.Mnemonico))
                {
                    result = "Tipo de perfil do poço correlação inválido";
                    break;
                }
            }

            return result;
        }
    }
}
