using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Cálculos.Base;

namespace SestWeb.Application.UseCases.CálculosUseCase.CálculoPerfis.CriarCálculo
{
    public class CriarCálculoPerfisOutput : UseCaseOutput<CriarCálculoPerfisStatus>
    {
        public CriarCálculoPerfisOutput()
        {
            
        }

        public ICálculo Cálculo { get; set; }

        public static CriarCálculoPerfisOutput CálculoPerfisCriado(ICálculo cálculo)
        {
            return new CriarCálculoPerfisOutput
            {
                Status = CriarCálculoPerfisStatus.CálculoPerfisCriado,
                Mensagem = "Cálculo de perfis criado com sucesso",
                Cálculo = cálculo
            };
        }

        public static CriarCálculoPerfisOutput CálculoPerfisNãoCriado(string msg)
        {
            return new CriarCálculoPerfisOutput
            {
                Status = CriarCálculoPerfisStatus.CálculoPerfisNãoCriado,
                Mensagem = $"Não foi possível criar cálculo de perfis. {msg}"
            };
        }
    }
}
