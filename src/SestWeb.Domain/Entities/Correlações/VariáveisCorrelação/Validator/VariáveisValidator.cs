using System;
using System.Collections.Generic;
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

namespace SestWeb.Domain.Entities.Correlações.VariáveisCorrelação.Validator
{
    public class VariáveisValidator : AbstractValidator<IVariáveis>, IVariáveisValidator
    {
        public VariáveisValidator()
        {
            RuleFor(v => v).Cascade(CascadeMode.StopOnFirstFailure).NotNull();

            RuleFor(vars => vars).Custom((vars, context) => ValidateVars(vars, context));
        }

        protected override bool PreValidate(ValidationContext<IVariáveis> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(typeof(IVariáveis).Name, $"Variáveis não pode ser null."));
                return false;
            }
            return true;
        }

        private void ValidateVars(IVariáveis vars, CustomContext context)
        {
            var variávelEmteste = string.Empty;
            try
            {
                var variáveisValidando = vars.Names;
                foreach (var variável in variáveisValidando)
                {
                    variávelEmteste = variável;

                    if (variável.Equals(String.Empty))
                    {
                        context.AddFailure($"Variável \"{variável}\" inválida - nome deve ser fornecido!");
                        continue;
                    }

                    if (NomeContémCaracteresInválidos(variável))
                    {
                        context.AddFailure($"Variável \"{variável}\" inválida - Nome contendo caracteres não permitidos!");
                        continue;
                    }

                    if (VerificarSeÉNumeral(variável))
                    {
                        context.AddFailure(
                            $"Variável \"{variável}\" inválida - Não é permitido nomes de variáveis com formato numérico!");
                        continue;
                    }

                    if (NomeCoincideComTipoDePerfil(variável))
                    {
                        context.AddFailure($"Variável \"{variável}\" inválida - Nome reservado para entrada de perfis!");
                        continue;
                    }

                    if (NomeCoincideComTokensDaCorrelação(variável))
                    {
                        context.AddFailure(
                            $"Variável \"{variável}\" inválida - Nome reservado para token de {EnumUtils.GetDescriptionFromEnumValue((TokensEnum) Enum.Parse(typeof(TokensEnum), variável))}.");
                        continue;
                    }

                    if (NomeCoincideComGrupoLitológico(variável))
                    {
                        context.AddFailure($"Variável \"{variável}\" inválida - Nome reservado para Grupo Litológico");
                        continue;
                    }

                    if (ParserNameValidation(variável, context))
                    {
                        vars.TryGetValue(variável, out double valorEmTeste);
                        ParserValueValidation(variável, valorEmTeste, context);
                    }
                }
            }
            catch (ParserException error)
            {
                var descrição =
                    EnumUtils.GetDescriptionFromEnumValue(
                        (MuParserNetWrapper.ParserErrorCodes)
                        Enum.Parse(typeof(MuParserNetWrapper.ParserErrorCodes), error.Code.ToString()));
                context.AddFailure($"{error.Expr} : {descrição} \"{error.Token}\" encontrado na posição {error.Pos}.");
            }
            catch (Exception exception)
            {
                context.AddFailure($"Erro ao validar a variável \"{variávelEmteste}\" - " + exception.Message);
                // TODO: (RCM) Investigar possíveis casos
            }
        }

        private static bool ParserNameValidation(string varName, CustomContext context)
        {
            try
            {
                var valorTeste = 2;
                using (var muParserNetWrapper = new MuParserNetWrapper("Validador_Variáveis"))
                {
                    var validationParser = muParserNetWrapper.Parser;
                    validationParser.Expr = varName;

                    validationParser.DefineVar(varName, valorTeste);

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
                context.AddFailure($"{error.Expr} : {descrição} \"{error.Token}\" encontrado na posição {error.Pos}.");
                return false;
            }
            catch (Exception exception)
            {
                context.AddFailure($"Erro ao validar a variável \"{varName}\" - " + exception.Message);
                return false;
                // TODO: (RCM) Investigar possíveis casos
            }
        }

        private static void ParserValueValidation(string varName, double varValue, CustomContext context)
        {
            try
            {
                if (varValue.Equals(0.0d))
                    return;

                using (var muParserNetWrapper = new MuParserNetWrapper("Validador_Variáveis"))
                {
                    var validationParser = muParserNetWrapper.Parser;
                    validationParser.Expr = varName;

                    validationParser.DefineVar(varName, varValue);

                    var validada = validationParser.Eval();

                    var error = Math.Abs(validada - varValue) / Math.Abs(varValue);
                    if (error > 0.00001) //0.001% de erro tolerado
                    {
                        context.AddFailure(
                            $"variável \"{varName}\" de valor \"{varValue}\" é inválida. Foi avaliada com valor: {validada}");
                    }
                }
            }
            catch (ParserException error)
            {
                var descrição =
                    EnumUtils.GetDescriptionFromEnumValue(
                        (MuParserNetWrapper.ParserErrorCodes)
                        Enum.Parse(typeof(MuParserNetWrapper.ParserErrorCodes), error.Code.ToString()));
                context.AddFailure($"{error.Expr} : {descrição} \"{error.Token}\" encontrado na posição {error.Pos}.");
            }
            catch (Exception exception)
            {
                context.AddFailure($"Erro ao validar a variável \"{varName}\" - " + exception.Message);
                // TODO: (RCM) Investigar possíveis casos
            }
        }

        private bool VerificarSeÉNumeral(string variável)
        {
            return double.TryParse(variável, out double valor);
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

        private bool NomeCoincideComTokensDaCorrelação(string nome)
        {
            var identificadores = Enum.GetNames(typeof(TokensEnum)).ToList();
            return identificadores.Any(i => string.Equals(i, nome));
        }

        private bool NomeCoincideComGrupoLitológico(string nome)
        {
            var gruposLitológicos = GrupoLitologico.GetNames();
            return gruposLitológicos.Any(gl => string.Equals(gl, nome));
        }

        private bool ExisteConstanteComNomeIgual(Dictionary<string, double> constantes, string nome)
        {
            if (constantes == null) return false;
            return constantes.ContainsKey(nome);
        }
    }
}
