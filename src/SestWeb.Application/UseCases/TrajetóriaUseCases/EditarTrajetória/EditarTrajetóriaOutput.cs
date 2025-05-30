using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.TrajetóriaUseCases.EditarTrajetória
{
    public class EditarTrajetóriaOutput : UseCaseOutput<EditarTrajetóriaStatus>
    {
        private EditarTrajetóriaOutput()
        {
        }

        public Trajetória Trajetória { get; set; }
        public List<PerfilBase> PerfisAlterados { get; set; }
        public List<Litologia> Litologias { get; set; }

        public static EditarTrajetóriaOutput TrajetóriaEditada(Trajetória trajetória, List<PerfilBase> perfisAlterados, List<Litologia> litologias)
        {
            return new EditarTrajetóriaOutput
            {
                Status = EditarTrajetóriaStatus.TrajetóriaEditada,
                Mensagem = "Trajetória editada com sucesso.",
                Trajetória = trajetória,
                PerfisAlterados = perfisAlterados,
                Litologias = litologias
            };
        }

        public static EditarTrajetóriaOutput PoçoNãoEncontrado(string id)
        {
            return new EditarTrajetóriaOutput
            {
                Status = EditarTrajetóriaStatus.TrajetóriaNãoEditada,
                Mensagem = $"Não foi possível editar a trajetória. Não foi possível encontrar poço com id {id}."
            };
        }

        public static EditarTrajetóriaOutput TrajetóriaNãoEditada(string mensagem)
        {
            return new EditarTrajetóriaOutput
            {
                Status = EditarTrajetóriaStatus.TrajetóriaNãoEditada,
                Mensagem = $"Não foi possível editar a trajetória. {mensagem}"
            };
        }
    }
}
