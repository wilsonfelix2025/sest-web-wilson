using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPropriedadesMecânicas.CriarCálculo
{
    public class CriarCálculoPropMecOutput : UseCaseOutput<CriarCálculoPropMecStatus>
    {

        public CriarCálculoPropMecOutput()
        {

        }

        public ICálculo Cálculo { get; set; }

        public static CriarCálculoPropMecOutput CálculoPorpMecCriado(ICálculo cálculo)
        {
            return new CriarCálculoPropMecOutput
            {
                Status = CriarCálculoPropMecStatus.CálculoPropMecCriado,
                Mensagem = "Cálculo de propriedades mecânicas criado com sucesso",
                Cálculo = cálculo
            };
        }

        public static CriarCálculoPropMecOutput CálculoPropMecNãoCriado(string msg)
        {
            return new CriarCálculoPropMecOutput
            {
                Status = CriarCálculoPropMecStatus.CálculoPropMecNãoCriado,
                Mensagem = $"Não foi possível criar cálculo de propriedades mecânicas. {msg}"
            };
        }
    }
}
