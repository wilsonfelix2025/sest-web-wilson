using System;
using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using SestWeb.Domain.Entities.Correlações.OrigemCorrelação;

namespace SestWeb.Domain.Entities.Correlações.Base.CreatingCorrelação
{
    public class CorrelaçãoEmCriaçãoValidator : AbstractValidator<CorrelaçãoEmCriação>, ICorrelaçãoEmCriaçãoValidator
    {
        public CorrelaçãoEmCriaçãoValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(corr => corr.Nome).NotNull().When(correlação => correlação != null)
                .WithMessage("Nome da correlação não pode ser null!");

            RuleFor(corr => corr.NomeAutor).NotNull().When(correlação => correlação != null)
                .WithMessage("Nome do autor não pode ser null!");

            RuleFor(corr => corr.ChaveAutor).NotNull().When(correlação => correlação != null)
                .WithMessage("Chave do autor não pode ser null!");

            RuleFor(corr => corr.DataCriação).NotNull().When(correlação => correlação != null)
                .WithMessage("Data de criação da correlação não pode ser null!");

            RuleFor(corr => corr.Descrição).NotNull().When(correlação => correlação != null)
                .WithMessage("Descrição da correlação não pode ser null!");

            RuleFor(corr => corr.Origem).NotNull().When(correlação => correlação != null)
                .WithMessage("Origem da correlação não pode ser null!");

            RuleFor(corr => corr.Expressão).NotNull().When(correlação => correlação != null)
                .WithMessage("Expressão da correlação não pode ser null!");

            RuleFor(corr => corr.Origem)
                .Must(origem => OrigemDeveSerVálida(origem))
                .When(correlação => correlação != null)
                .WithMessage(
                    $"Origem da correlação deve ser uma das seguintes opções: {"\n"}{string.Join("\n", Enum.GetNames(typeof(Origem)))}!");//string.Join("\n", result.Errors)

            RuleFor(corr => corr.DataCriação)
                .Must(data => DataDeveSerVálida(data))
                .When(correlação => correlação != null)
                .WithMessage("Data de criação da correlação {PropertyValue} deve estar no formato dd/MM/yyyy !");

            RuleFor(corr => corr.Expressão).NotNull().Custom((expression, context) => VariáveisIdentificadasDevemPossuirValorComFormatoNumérico(expression, context));

            RuleFor(corr => corr.Expressão).NotNull().Custom((expression, context) => ConstantesIdentificadasDevemPossuirValorComFormatoNumérico(expression, context));
        }

        protected override bool PreValidate(ValidationContext<CorrelaçãoEmCriação> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(typeof(CorrelaçãoEmCriação).Name,
                    "Correlação não pode ser null!"));
                return false;
            }

            return true;
        }

        private bool OrigemDeveSerVálida(string origem)
        {
            return Enum.TryParse(origem, out Origem value);
        }

        private bool DataDeveSerVálida(string data)
        {
            return DateTime.TryParseExact(data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime dt);
        }

        private void VariáveisIdentificadasDevemPossuirValorComFormatoNumérico(string expressão, CustomContext context)
        {
            string pattern = @"\bvar\b\s*(?<Nome>\b[a-zA-Z]\w*)\s*=\s*(?<Valor>\d+\.?\d*)";

            Regex regex = new Regex(pattern);

            foreach (Match match in regex.Matches(expressão))
            {
                string name = match.Groups["Nome"].Value;
                string value = match.Groups["Valor"].Value;

                if (!double.TryParse(value, NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out double valor))
                {
                    context.AddFailure($"Variável: {name} não possui valor: {value} em formato numérico!");
                }
            }
        }

        private void ConstantesIdentificadasDevemPossuirValorComFormatoNumérico(string expressão, CustomContext context)
        {
            string pattern = @"\bconst\b\s*(?<Nome>\b[a-zA-Z]\w*)\s*=\s*(?<Valor>\d+\.?\d*)";

            Regex regex = new Regex(pattern);

            foreach (Match match in regex.Matches(expressão))
            {
                string name = match.Groups["Nome"].Value;
                string value = match.Groups["Valor"].Value;

                if (!double.TryParse(value, NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out double valor))
                {
                    context.AddFailure($"Constante: {name} não possui valor: {value} em formato numérico!");
                }
            }
        }
    }
}
