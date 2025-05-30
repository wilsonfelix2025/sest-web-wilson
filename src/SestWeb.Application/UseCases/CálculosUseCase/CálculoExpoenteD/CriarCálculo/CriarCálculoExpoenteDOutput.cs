using System;
using System.Collections.Generic;
using System.Text;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoExpoenteD.CriarCálculo
{
    public class CriarCálculoExpoenteDOutput : UseCaseOutput<CriarCálculoExpoenteDStatus>
    {
        public CriarCálculoExpoenteDOutput()
        {

        }

        public ICálculo Cálculo { get; set; }

        public static CriarCálculoExpoenteDOutput CálculoExpoenteDCriado(ICálculo cálculo)
        {
            return new CriarCálculoExpoenteDOutput
            {
                Status = CriarCálculoExpoenteDStatus.CálculoExpoenteDCriado,
                Mensagem = "Cálculo de ExpoenteD criado com sucesso",
                Cálculo = cálculo
            };
        }

        public static CriarCálculoExpoenteDOutput CálculoExpoenteDNãoCriado(string msg)
        {
            return new CriarCálculoExpoenteDOutput
            {
                Status = CriarCálculoExpoenteDStatus.CálculoExpoenteDNãoCriado,
                Mensagem = $"Não foi possível criar cálculo de ExpoenteD. {msg}"
            };
        }
    }
}
