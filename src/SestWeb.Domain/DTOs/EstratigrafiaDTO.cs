using System.Collections.Generic;

namespace SestWeb.Domain.DTOs
{
    public class EstratigrafiaDTO
    {
        public Dictionary<string, List<ItemEstratigrafiaDTO>> Itens { get; } = new Dictionary<string, List<ItemEstratigrafiaDTO>>();

        public bool Adicionar(string tipo, string pm, string sigla, string descrição, string idade)
        {
            var item = new ItemEstratigrafiaDTO(pm, sigla, descrição, idade);

            if (!Itens.ContainsKey(tipo))
            {
                var lista = new List<ItemEstratigrafiaDTO> { item };
                Itens.Add(tipo, lista);
                return true;
            }

            if (Itens[tipo].Contains(item))
                return false;

            Itens[tipo].Add(item);
            return true;
        }
    }
}
