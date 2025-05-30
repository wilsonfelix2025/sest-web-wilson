using System.Collections.Generic;
using System.Globalization;
using FluentValidation;
using SestWeb.Domain.DTOs;
using SestWeb.Domain.Enums;

namespace SestWeb.Domain.Validadores.DTO
{
    internal class PoçoDTOValidator : AbstractValidator<PoçoDTO>
    {
        public PoçoDTOValidator(List<string> nomesPerfisExistentes, List<DadosSelecionadosEnum> dadosSelecionados, bool existeLitologia, bool existePerfil)
        {
            When(c => dadosSelecionados.Contains(DadosSelecionadosEnum.DadosGerais), () =>
            {
                When(c => c.DadosGerais.Identificação != null, () =>
                    {
                        RuleFor(p => p.DadosGerais.Identificação.NomePoço).NotEmpty();

                    });
                #region Geometria
                When(c => c.DadosGerais.Geometria != null, () =>
                {

                    RuleFor(p => p.DadosGerais.Geometria.MesaRotativa)
                    .Custom((x, context) =>
                    {
                        if ((!(double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) || value < 0))
                        {
                            context.AddFailure($"{x} não é um número válido para mesa rotativa ou é menor que 0");
                        }
                    });


                    When(p => p.DadosGerais.Geometria.OnShore.Elevação?.Length > 0, () =>
                    {
                        RuleFor(p => p.DadosGerais.Geometria.OnShore.Elevação).Custom((x, context) =>
                        {
                            if ((!(double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) ||
                                 value < 0))
                            {
                                context.AddFailure($"{x} não é um número válido para elevação ou é menor que 0");
                            }
                        });

                        RuleFor(p => p.DadosGerais.Geometria.OnShore.LençolFreático).
                            Custom((x, context) =>
                            {
                                if ((!(double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) || value < 0))
                                {
                                    context.AddFailure($"{x} não é um número válido para lençol freático ou é menor que 0");
                                }
                            });

                        RuleFor(p => p.DadosGerais.Geometria.OnShore.AlturaDeAntePoço).
                            Custom((x, context) =>
                            {
                                if ((!(double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) || value < 0))
                                {
                                    context.AddFailure($"{x} não é um número válido para altura de ante poço ou é menor que 0");
                                }
                            });


                    });


                    When(p => p.DadosGerais.Geometria.OffShore.LaminaDagua?.Length > 0, () =>
                    {
                        RuleFor(p => p.DadosGerais.Geometria.OffShore.LaminaDagua).Custom((x, context) =>
                        {
                            if ((!(double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) ||
                                 value < 0))
                            {
                                context.AddFailure($"{x} não é um número válido para lâmina d'agua ou é menor que 0");
                            }
                        });
                    });

                    When(p => p.DadosGerais.Geometria.Coordenadas.UtMx?.Length > 0, () =>
                    {
                        RuleFor(p => p.DadosGerais.Geometria.Coordenadas.UtMx).Custom((x, context) =>
                        {
                            if ((!(double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) ||
                                 value < 0))
                            {
                                context.AddFailure($"{x} não é um número válido para coordenada UtMx ou é menor que 0");
                            }
                        });
                    });

                    When(p => p.DadosGerais.Geometria.Coordenadas.UtMy?.Length > 0, () =>
                    {
                        RuleFor(p => p.DadosGerais.Geometria.Coordenadas.UtMy).Custom((x, context) =>
                        {
                            if ((!(double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) ||
                                 value < 0))
                            {
                                context.AddFailure($"{x} não é um número válido para coordenada UtMy ou é menor que 0");
                            }
                        });
                    });

                });
                #endregion

                #region Area

                When(c => c.DadosGerais.Area != null, () =>
                {


                    When(p => p.DadosGerais.Area.DensidadeSuperficie?.Length > 0, () =>
                {
                    RuleFor(p => p.DadosGerais.Area.DensidadeSuperficie)
                        .Custom((x, context) =>
                        {
                            if ((!(double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) ||
                                 value < 0))
                            {
                                context.AddFailure(
                                    $"{x} não é um número válido para densidade de superfície ou é menor que 0");
                            }
                        });
                });

                    When(p => p.DadosGerais.Area.DensidadeAguaMar?.Length > 0, () =>
                    {
                        RuleFor(p => p.DadosGerais.Area.DensidadeAguaMar)
                            .Custom((x, context) =>
                            {
                                if ((!(double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) ||
                                     value < 0))
                                {
                                    context.AddFailure(
                                        $"{x} não é um número válido para densidade de água do mar ou é menor que 0");
                                }
                            });
                    });

                    When(p => p.DadosGerais.Area.SonicoSuperficie?.Length > 0, () =>
                    {
                        RuleFor(p => p.DadosGerais.Area.SonicoSuperficie)
                            .Custom((x, context) =>
                            {
                                if ((!(double.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)) ||
                                     value < 0))
                                {
                                    context.AddFailure($"{x} não é um número válido para sônico superfície ou é menor que 0");
                                }
                            });
                    });
                });

                #endregion
            });

            When(c => dadosSelecionados.Contains(DadosSelecionadosEnum.Sapatas), () =>
            {
                RuleForEach(p => p.Sapatas).SetValidator(new SapataDTOValidator());
            });

            When(c => existeLitologia == true, () =>
            {
                RuleForEach(p => p.Litologias).SetValidator(new LitologiaDTOValidator());
            });

            When(c => dadosSelecionados.Contains(DadosSelecionadosEnum.Objetivos), () =>
            {
                RuleForEach(p => p.Objetivos).SetValidator(new ObjetivoDTOValidator());
            });

            When(c => dadosSelecionados.Contains(DadosSelecionadosEnum.Trajetória), () =>
            {
                RuleFor(p => p.Trajetória).SetValidator(new TrajetoriaDTOValidator());
            });

            When(c => existePerfil == true, () =>
            {
                RuleForEach(p => p.Perfis).SetValidator(new PerfilDTOValidator(nomesPerfisExistentes));
            });
        }

    }
}
