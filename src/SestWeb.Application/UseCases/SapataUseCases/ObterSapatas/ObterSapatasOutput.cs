using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Poço.Sapatas;

namespace SestWeb.Application.UseCases.SapataUseCases.ObterSapatas
{
    public class ObterSapatasOutput : UseCaseOutput<ObterSapatasStatus>
    {
        private ObterSapatasOutput()
        {
        }

        public IReadOnlyCollection<Sapata> Sapatas { get; set; }

        public static ObterSapatasOutput SapatasObtidas(IReadOnlyCollection<Sapata> sapatas)
        {
            return new ObterSapatasOutput
            {
                Sapatas = sapatas,
                Status = ObterSapatasStatus.SapatasObtidas,
                Mensagem = "Sapatas obtidas com sucesso."
            };
        }

        public static ObterSapatasOutput PoçoNãoEncontrado(string id)
        {
            return new ObterSapatasOutput
            {
                Status = ObterSapatasStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }

        public static ObterSapatasOutput SapatasNãoObtidas(string mensagem)
        {
            return new ObterSapatasOutput
            {
                Status = ObterSapatasStatus.SapatasNãoObtidas,
                Mensagem = $"Não foi possível obter sapatas. {mensagem}"
            };
        }
    }
}