using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using SestWeb.Domain.Entities.Correlações.ConstantesCorrelação;
using SestWeb.Domain.Entities.Correlações.ConstantesCorrelação.Validator;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.TiposVálidosCorrelação;
using SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper;
using SestWeb.Domain.Entities.Correlações.PerfisEntradaCorrelação.Validator;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação;
using SestWeb.Domain.Entities.Correlações.PerfisSaídaCorrelação.Validator;
using SestWeb.Domain.Entities.Correlações.TokensCorrelação;
using SestWeb.Domain.Entities.Correlações.VariáveisCorrelação;
using SestWeb.Domain.Entities.Correlações.VariáveisCorrelação.Validator;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.Validator
{
    public class ExpressionValidator : AbstractValidator<IExpressão>, IExpressionValidator
    {
        public ExpressionValidator(IVariáveisValidator variáveisValidator, IConstantesValidator constantesValidator, IPerfisSaídaValidator perfisSaídaValidator)
        {
            try
            {
                ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

                RuleFor(expression => expression.Bruta).NotNull().WithMessage("Expressão Bruta não pode ser null!");

                RuleFor(expression => expression.Normalizada).NotNull().WithMessage("Expressão Normalizada não pode ser null!");

                RuleFor(expression => expression.Constantes).SetValidator(constantesValidator as AbstractValidator<IConstantes>).When(expression => expression != null);

                RuleFor(expression => expression.Variáveis).SetValidator(variáveisValidator as AbstractValidator<IVariáveis>).When(expression => expression != null);

                RuleFor(expression => expression.PerfisSaída).SetValidator(perfisSaídaValidator as AbstractValidator<IPerfisSaída>).When(expression => expression != null);

                RuleFor(expression => expression.PerfisEntrada).SetValidator(new PerfisEntradaValidator()).When(expression => expression != null);

                RuleFor(expression => expression).Custom((expression, context) => ValidarConstantesEVariáveisComMesmoNome(expression, context));

                RuleFor(expression => expression).Custom((expression, context) => ValidarExpressãoNoParser(expression, context));
            }
            catch (Exception e)
            {
                Console.WriteLine($"ExpressionValidator: Erro ao validar a expressão : {e.Message}.");
            }
        }

        protected override bool PreValidate(ValidationContext<IExpressão> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(typeof(IExpressão).Name, "Expressão não pode ser null!"));
                return false;
            }
            return true;
        }

        private void ValidarConstantesEVariáveisComMesmoNome(IExpressão expressão, CustomContext context)
        {
            if (expressão==null || expressão.Variáveis==null || expressão.Constantes==null || !expressão.Variáveis.Any() || !expressão.Constantes.Any())
                return;

            var variáveis = expressão.Variáveis.Names;
            var constantes = expressão.Constantes.Names;

            List<string> nosmesComuns = (from variável in variáveis from constante in constantes where variável.Equals(constante) select variável).ToList();

            if (nosmesComuns.Any())
            {
                string nomes = string.Join(", ", nosmesComuns);
                context.AddFailure($"A expressão \"{expressão.Bruta}\" contém constantes e variáveis com o mesmo nome: {nomes}.");
            }
        }

        private void ValidarExpressãoNoParser(IExpressão expressão, CustomContext context)
        {
            const double valorDeValidação = 2;

            if (expressão == null || string.IsNullOrEmpty(expressão.Bruta) || string.IsNullOrWhiteSpace(expressão.Bruta))
            {
                context.AddFailure($"Expressão não declarada.");
                return;
            }
            Validar(expressão, valorDeValidação, context);
        }

        private void Validar(IExpressão expressão, double valorDeValidação, CustomContext context)
        {
            try
            {
                using (var parserWrapper = new MuParserNetWrapper("Validador_Expressão"))
                {
                    var validationParser = parserWrapper.Parser;
                    validationParser.Expr = expressão.Normalizada;

                    // Criação de variável para o GRUPO_LITOLOGICO
                    CriarVariávelDeGrupoLitológicoNoParser(expressão, valorDeValidação, validationParser);

                    // Criação de variável para PROFUNDIDADE
                    CriarVariávelParaProfundidadeNoParser(expressão, valorDeValidação, validationParser);

                    // Criação de variável para cada Perfil de entrada
                    CriarVariáveisParaOsPerfisDeEntradaNoParser(expressão, valorDeValidação, validationParser);

                    // Criação de variável para cada variável da expressão
                    CriarVariáveisParaAsVariáveisNoParser(expressão, validationParser);

                    // Criação de variável para profundidade Inicial
                    CriarVariávelDeProfundidadeInicialNoParser(expressão, valorDeValidação, validationParser);

                    // Criação de variável para Rhob Inicial
                    CriarVariávelDeRhobInicialNoParser(expressão, valorDeValidação, validationParser);

                    // Criação de variável para Step Fixo
                    CriarVariávelDeStepFixoNoParser(expressão, valorDeValidação, validationParser);

                    // Criação de variável para Densidade da água do mar
                    CriarVariávelDeDensidadeÁguaMarNoParser(expressão, valorDeValidação, validationParser);

                    // Criação de variável para Lâmina D'água
                    CriarVariávelDeLâminaDÁguaNoParser(expressão, valorDeValidação, validationParser);

                    // Criação de variável para Mesa Rotativa
                    CriarVariávelDeMesaRotativaNoParser(expressão, valorDeValidação, validationParser);

                    // Criação de variável para Altura de Antepoço
                    CriarVariávelDeAlturaDeAntePoçoNoParser(expressão, valorDeValidação, validationParser);

                    // Criação de variável para Categoria do poço
                    CriarVariávelDeCategoriaDoPocoNoParser(expressão, valorDeValidação, validationParser);

                    // Criação de variável para Poço OffShore
                    CriarVariávelDePoçoOffShoreNoParser(expressão, valorDeValidação, validationParser);

                    // Criação de variável para Poço OnShore
                    CriarVariávelDePoçoOnShoreNoParser(expressão, valorDeValidação, validationParser);

                    // // Criação de variável para cada constante da expressão
                    // Constantes estão definidas como variáveis porque o parser não permite atribuição de valores à constantes, na expressão
                    CriarVariáveisParaAsConstantesNoParser(expressão, validationParser);

                    validationParser.Eval();
                }
            }
            catch (ParserException error)
            {
                var descrição =
                    EnumUtils.GetDescriptionFromEnumValue(
                        (MuParserNetWrapper.ParserErrorCodes)
                            Enum.Parse(typeof(MuParserNetWrapper.ParserErrorCodes), error.Code.ToString()));
                context.AddFailure($"Expressão: {error.Expr} : {descrição}  encontrado na posição {error.Pos}");
            }
            catch (Exception exception)
            {
                // TODO: (RCM) Investigar possíveis casos
                context.AddFailure($"Erro ao validar a expressão \"{expressão} : " + exception.Message);
            }
        }

        private void CriarFábricaParaAsVaviáveisInternasNoParser(double valorDeValidação, Parser validationParser)
        {
            validationParser.SetVarFactory((name, userdata) =>
            {
                // cria o objeto com o valor passado na função
                return new ParserVariable(name, (double)userdata);
            }, 0.0);
        }

        private static void CriarVariáveisParaAsConstantesNoParser(IExpressão expressão, Parser validationParser)
        {
            if (expressão.Constantes != null && expressão.Constantes.Any())
            {
                foreach (var constante in expressão.Constantes.Names)
                {
                    if (Regex.IsMatch(expressão.Normalizada, constante))
                    {
                        if(expressão.Constantes.TryGetValue(constante, out double valor))
                            validationParser.DefineVar(constante, valor);
                    }
                }
            }
        }

        private static void CriarVariáveisParaAsVariáveisNoParser(IExpressão expressão, Parser validationParser)
        {
            if (expressão.Variáveis != null && expressão.Variáveis.Any())
            {
                foreach (var variável in expressão.Variáveis.Names)
                {
                    if (Regex.IsMatch(expressão.Normalizada, variável))
                    {
                        if(expressão.Variáveis.TryGetValue(variável, out double valor))
                            validationParser.DefineVar(variável, valor);
                    }
                }
            }
        }

        private static void CriarVariáveisParaOsPerfisDeEntradaNoParser(IExpressão expressão, double valorDeValidação, Parser validationParser)
        {
            foreach (var tipo in TiposVálidos.Perfis)
            {
                if (new Regex($@"\b{tipo}\b").IsMatch(expressão.Normalizada))
                {
                    validationParser.DefineVar(tipo, valorDeValidação);
                }
            }
        }

        private static void CriarVariávelParaProfundidadeNoParser(IExpressão expressão, double valorDeValidação, Parser validationParser)
        {
            if (expressão.Analitics.TemCálculoPorTrecho)
            {
                validationParser.DefineVar(Tokens.TokenDeCálculoPorTrecho, valorDeValidação);
            }
        }

        private static void CriarVariávelDeGrupoLitológicoNoParser(IExpressão expressão, double valorDeValidação, Parser validationParser)
        {
            if (expressão.Analitics.TemCálculoPorGrupoLitológico)
            {
                validationParser.DefineVar(Tokens.TokenDeGrupoLitológico, valorDeValidação);
            }
        }

        private static void CriarVariávelDeProfundidadeInicialNoParser(IExpressão expressão, double valorDeValidação, Parser validationParser)
        {
            if (expressão.Analitics.TemCálculoApartirDeProfundidadeInicial)
            {
                validationParser.DefineVar(Tokens.TokenDeProfundidadeInicial, valorDeValidação);
            }
        }

        private static void CriarVariávelDeRhobInicialNoParser(IExpressão expressão, double valorDeValidação, Parser validationParser)
        {
            if (expressão.Analitics.TemCálculoComRhobInicial)
            {
                validationParser.DefineVar(Tokens.TokenDeRhobInicial, valorDeValidação);
            }
        }

        private static void CriarVariávelDeStepFixoNoParser(IExpressão expressão, double valorDeValidação, Parser validationParser)
        {
            if (expressão.Analitics.TemCálculoComStepFixo)
            {
                validationParser.DefineVar(Tokens.TokenDeCálculoComStepFixo, valorDeValidação);
            }
        }

        private static void CriarVariávelDeDensidadeÁguaMarNoParser(IExpressão expressão, double valorDeValidação, Parser validationParser)
        {
            if (expressão.Analitics.TemCálculoComDensidadeAguaDoMar)
            {
                validationParser.DefineVar(Tokens.TokenDeDensidadeÁguaMar, valorDeValidação);
            }
        }

        private static void CriarVariávelDeLâminaDÁguaNoParser(IExpressão expressão, double valorDeValidação, Parser validationParser)
        {
            if (expressão.Analitics.TemCálculoComLâminaDAgua)
            {
                validationParser.DefineVar(Tokens.TokenDeLâminaDAgua, valorDeValidação);
            }
        }

        private static void CriarVariávelDeMesaRotativaNoParser(IExpressão expressão, double valorDeValidação, Parser validationParser)
        {
            if (expressão.Analitics.TemCálculoComMesaRotativa)
            {
                validationParser.DefineVar(Tokens.TokenDeMesaRotativa, valorDeValidação);
            }
        }

        private static void CriarVariávelDeAlturaDeAntePoçoNoParser(IExpressão expressão, double valorDeValidação, Parser validationParser)
        {
            if (expressão.Analitics.TemCálculoComAlturaDeAntepoço)
            {
                validationParser.DefineVar(Tokens.TokenDeAlturaDeAntepoço, valorDeValidação);
            }
        }

        private static void CriarVariávelDeCategoriaDoPocoNoParser(IExpressão expressão, double valorDeValidação, Parser validationParser)
        {
            if (expressão.Analitics.TemCálculoComCategoriaDoPoco)
            {
                validationParser.DefineVar(Tokens.TokenDeCategoriaDoPoco, valorDeValidação);
            }
        }

        private static void CriarVariávelDePoçoOffShoreNoParser(IExpressão expressão, double valorDeValidação, Parser validationParser)
        {
            if (expressão.Analitics.TemCálculoComPoçoOffShore)
            {
                validationParser.DefineVar(Tokens.TokenDePoçoOffShore, valorDeValidação);
            }
        }

        private static void CriarVariávelDePoçoOnShoreNoParser(IExpressão expressão, double valorDeValidação, Parser validationParser)
        {
            if (expressão.Analitics.TemCálculoComPoçoOnShore)
            {
                validationParser.DefineVar(Tokens.TokenDePoçoOnShore, valorDeValidação);
            }
        }
    }
}
