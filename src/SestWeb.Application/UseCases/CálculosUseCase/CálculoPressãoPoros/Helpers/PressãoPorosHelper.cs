using SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.Base;
using SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.ReservatórioInput;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Base;
using SestWeb.Domain.Entities.Cálculos.PressãoPoros.Reservatório;
using SestWeb.Domain.Entities.Correlações.ParâmetroCorrelação;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.Poço;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPressãoPoros.Helpers
{
    public class PressãoPorosHelper
    {
        public static DadosReservatório PreencherDadosReservatório(DadosReservatórioInput inputReservatório)
        {
            if (inputReservatório != null)
            {
                return new DadosReservatório
                {
                    Nome = inputReservatório.Nome,
                    Interesse = new InteresseReservatório
                    {
                        Base = inputReservatório.Interesse.Base,
                        Topo = inputReservatório.Interesse.Topo
                    },
                    Referencia = new ReferênciaReservatório
                    {
                        Cota = inputReservatório.Referencia.Cota,
                        Pp = inputReservatório.Referencia.Pp,
                        Poco = inputReservatório.Referencia.Poco,
                        Contatos = new ContatosReservatório
                        {
                            GasOleo = inputReservatório.Referencia.Contatos.GasOleo,
                            OleoAgua = inputReservatório.Referencia.Contatos.OleoAgua
                        },
                        Gradiente = new GradienteReservatório
                        {
                            Agua = inputReservatório.Referencia.Gradiente.Agua,
                            Gas = inputReservatório.Referencia.Gradiente.Gas,
                            Oleo = inputReservatório.Referencia.Gradiente.Oleo
                        }
                    }
                };
            }

            return null;
        }

        public static void ExtrairCorrelações(CálculoPressãoPorosInput input, List<ParâmetroCorrelação> parâmetrosCorrelação)
        {
            if (double.TryParse(input.Gn, NumberStyles.Any, CultureInfo.InvariantCulture, out var gn))
            {
                parâmetrosCorrelação.Add(new ParâmetroCorrelação("Gn", gn));
            }

            if (double.TryParse(input.Exp, NumberStyles.Any, CultureInfo.InvariantCulture, out var exp))
            {
                parâmetrosCorrelação.Add(new ParâmetroCorrelação("Exp", exp));
            }

            if (double.TryParse(input.Gpph, NumberStyles.Any, CultureInfo.InvariantCulture, out var gpph))
            {
                parâmetrosCorrelação.Add(new ParâmetroCorrelação("Gn", gpph));
            }
        }
    }
}
