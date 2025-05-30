using SestWeb.Domain.DTOs;
using System.Collections.Generic;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;

namespace SestWeb.Domain.Factories
{
    public class PoçoDTOFactory
    {
        public PoçoDTO CriarPoçoDTO(string id, string nome, TipoPoço tipo)
        {
            var poço = new PoçoDTO();

            return poço;
        }

        public static PoçoDTO CriarPoçoDTO(TrajetóriaDTO trajetória, List<LitologiaDTO> litologias, DadosGeraisDTO dadosGerais, List<PerfilDTO> perfis, List<SapataDTO> sapatas
            , List<ObjetivoDTO> objetivos, EstratigrafiaDTO estratigrafia, List<RegistroDTO> registros, List<RegistroDTO> eventos)
        {
            var poço = new PoçoDTO
            {
                DadosGerais = dadosGerais,
                Estratigrafia = estratigrafia,
                Litologias = litologias,
                Objetivos = objetivos,
                Perfis = perfis,
                Sapatas = sapatas,
                Trajetória = trajetória,
                Registros = registros,
                Eventos = eventos
            };

            return poço;
        }
    }
}
