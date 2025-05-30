using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.Poço.Objetivos;

namespace SestWeb.Application.UseCases.ObjetivoUseCases.ObterObjetivos
{
    public class ObterObjetivosOutput : UseCaseOutput<ObterObjetivosStatus>
    {
        private ObterObjetivosOutput()
        {
        }

        public IReadOnlyCollection<Objetivo> Objetivos { get; set; }

        public static ObterObjetivosOutput ObjetivosObtidos(IReadOnlyCollection<Objetivo> objetivos)
        {
            return new ObterObjetivosOutput
            {
                Objetivos = objetivos,
                Status = ObterObjetivosStatus.ObjetivosObtidos,
                Mensagem = "Objetivos obtidos com sucesso."
            };
        }

        public static ObterObjetivosOutput PoçoNãoEncontrado(string id)
        {
            return new ObterObjetivosOutput
            {
                Status = ObterObjetivosStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }

        public static ObterObjetivosOutput ObjetivosNãoObtidos(string mensagem)
        {
            return new ObterObjetivosOutput
            {
                Status = ObterObjetivosStatus.ObjetivosNãoObtidos,
                Mensagem = $"Não foi possível obter objetivos. {mensagem}"
            };
        }
    }
}