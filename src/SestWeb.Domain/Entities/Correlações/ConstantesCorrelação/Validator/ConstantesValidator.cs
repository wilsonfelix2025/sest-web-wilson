using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.TiposVálidosCorrelação;
using SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper;
using SestWeb.Domain.Entities.Correlações.TokensCorrelação;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Helpers;

namespace SestWeb.Domain.Entities.Correlações.ConstantesCorrelação.Validator
{
    public class ConstantesValidator : AbstractValidator<IConstantes>, IConstantesValidator
    {
        public ConstantesValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(consts => consts).NotNull();

            RuleFor(consts => consts).Custom((consts, context) => ValidateConst(consts, context));
        }

        protected override bool PreValidate(ValidationContext<IConstantes> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(typeof(IConstantes).Name, $"Constantes não pode ser null."));
                return false;
            }
            return true;
        }

        private void ValidateConst(IConstantes consts, CustomContext context)
        {
            var constanteValidando = string.Empty;
            try
            {
                var constantesValidando = consts.Names;

                foreach (var constante in constantesValidando)
                {
                    constanteValidando = constante;

                    if (constante.Equals(String.Empty))
                    {
                        context.AddFailure($"Constante \"{constante}\" inválida - nome deve ser fornecido!");
                        continue;
                    }

                    if (NomeContémCaracteresInválidos(constante))
                    {
                        context.AddFailure($"Constante \"{constante}\" inválida - Nome contendo caracteres não permitidos!");
                        continue;
                    }

                    if (VerificarSeÉNumeral(constante))
                    {
                        context.AddFailure(
                            $"Constante \"{constante}\" inválida - Não é permitido nomes de constantes com formato numérico!");
                        continue;
                    }

                    if (NomeCoincideComTipoDePerfil(constante))
                    {
                        context.AddFailure($"Constante \"{constante}\" inválida - Nome reservado para entrada de perfis!");
                        continue;
                    }

                    if (NomeCoincideComIdentificadoresDaCorrelação(constante))
                    {
                        context.AddFailure(
                            $"Constante \"{constante}\" inválida - Nome reservado para token de {EnumUtils.GetDescriptionFromEnumValue((TokensEnum) Enum.Parse(typeof(TokensEnum), constante))}.");
                        continue;
                    }

                    if (NomeCoincideComGrupoLitológico(constante))
                    {
                        context.AddFailure($"Constante \"{constante}\" inválida - Nome reservado para Grupo Litológico");
                        continue;
                    }

                    if (ParserValidation(constante, context))
                    {
                        consts.TryGetValue(constante, out double valorEmTeste);
                        ParserValueValidation(constante, valorEmTeste, context);
                    }
                }
            }
            catch (ParserException error)
            {
                var descrição =
                    EnumUtils.GetDescriptionFromEnumValue(
                        (MuParserNetWrapper.ParserErrorCodes)
                        Enum.Parse(typeof(MuParserNetWrapper.ParserErrorCodes), error.Code.ToString()));
                context.AddFailure($"{error.Expr} : {descrição}  encontrado na posição {error.Pos}.");
            }
            catch (Exception exception)
            {
                context.AddFailure($"Erro ao validar a constante \"{constanteValidando}\" - " + exception.Message);
                // TODO: (RCM) Investigar possíveis casos
            }
        }

        private static bool ParserValidation(string constante, CustomContext context)
        {
            try
            {
                var valorTeste = 2;
                using (var parserWrapper = new MuParserNetWrapper("Validador_Constantes"))
                {
                    var validationParser = parserWrapper.Parser;
                    validationParser.Expr = constante;

                    // define a constante
                    validationParser.DefineConst(constante, valorTeste);
                    validationParser.Eval();
                }

                return true;
            }
            catch (ParserException error)
            {
                var descrição =
                    EnumUtils.GetDescriptionFromEnumValue(
                        (MuParserNetWrapper.ParserErrorCodes)
                        Enum.Parse(typeof(MuParserNetWrapper.ParserErrorCodes), error.Code.ToString()));
                context.AddFailure($"{error.Expr} : {descrição}  encontrado na posição {error.Pos}.");
                return false;
            }
            catch (Exception exception)
            {
                context.AddFailure($"Erro ao validar a constante \"{constante}\" - " + exception.Message);
                return false;
                // TODO: (RCM) Investigar possíveis casos
            }
        }

        private static void ParserValueValidation(string constName, double constValue, CustomContext context)
        {
            try
            {
                if (constValue.Equals(0.0d))
                    return;

                using (var muParserNetWrapper = new MuParserNetWrapper("Validador_Constantes"))
                {
                    var validationParser = muParserNetWrapper.Parser;
                    validationParser.Expr = constName;

                    validationParser.DefineConst(constName, constValue);

                    var validada = validationParser.Eval();

                    var error = Math.Abs(validada - constValue) / Math.Abs(constValue);
                    if (error > 0.00001) //0.001% de erro tolerado
                    {
                        context.AddFailure(
                            $"Constante \"{constName}\" de valor \"{constValue}\" é inválida. Foi avaliada com valor: {validada}");
                    }
                }
            }
            catch (ParserException error)
            {
                var descrição =
                    EnumUtils.GetDescriptionFromEnumValue(
                        (MuParserNetWrapper.ParserErrorCodes)
                        Enum.Parse(typeof(MuParserNetWrapper.ParserErrorCodes), error.Code.ToString()));
                context.AddFailure($"{error.Expr} : {descrição}  encontrado na posição {error.Pos}.");
            }
            catch (Exception exception)
            {
                context.AddFailure($"Erro ao validar a constante \"{constName}\" - " + exception.Message);
                // TODO: (RCM) Investigar possíveis casos
            }
        }

        private bool VerificarSeÉNumeral(string variável)
        {
            return double.TryParse(variável, NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out double valor);
        }

        private bool NomeContémCaracteresInválidos(string nome)
        {
            Regex namePattern = new Regex(@"^[\w]*$");
            return !namePattern.IsMatch(nome);
        }

        private bool NomeCoincideComTipoDePerfil(string nome)
        {
            return TiposVálidos.Perfis.Any(p => string.Equals(p, nome));
        }

        private bool NomeCoincideComIdentificadoresDaCorrelação(string nome)
        {
            var identificadores = Enum.GetNames(typeof(TokensEnum)).ToList();
            return identificadores.Any(i => string.Equals(i, nome));
        }

        private bool NomeCoincideComGrupoLitológico(string nome)
        {
            var gruposLitológicos = GrupoLitologico.GetNames();
            return gruposLitológicos.Any(gl => string.Equals(gl, nome));
        }

        private bool ExisteVariávelComNomeIgual(Dictionary<string, double> variáveis, string nome)
        {
            if (variáveis == null) return false;
            return variáveis.ContainsKey(nome);
        }
    }
}
