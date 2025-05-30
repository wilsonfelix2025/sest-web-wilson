using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.LitologiaUseCases.EditarLitologia
{
    public class EditarLitologiaOutput : UseCaseOutput<EditarLitologiaStatus>
    {
        private EditarLitologiaOutput()
        {
        }

        public Litologia Litologia { get; set; }
        public List<PerfilBase> PerfisAlterados { get; set; }

        public static EditarLitologiaOutput LitologiaEditada(Litologia litologia, List<PerfilBase> perfisAlterados)
        {
            return new EditarLitologiaOutput
            {
                Status = EditarLitologiaStatus.LitologiaEditada,
                Mensagem = "Litologia editada com sucesso.",
                Litologia = litologia,
                PerfisAlterados = perfisAlterados
            };
        }

        public static EditarLitologiaOutput PoçoNãoEncontrado(string idPoço) {
            return new EditarLitologiaOutput
            {
                Status = EditarLitologiaStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com ID {idPoço}."
            };
        }

        public static EditarLitologiaOutput LitologiaNãoEncontrada(string tipoLitologia)
        {
            return new EditarLitologiaOutput
            {
                Status = EditarLitologiaStatus.LitologiaNãoEncontrada,
                Mensagem = $"Não foi possível encontrar litologia do tipo '{tipoLitologia}'."
            };
        }

        public static EditarLitologiaOutput LitologiaNãoEditada(string mensagem = "")
        {
            return new EditarLitologiaOutput
            {
                Status = EditarLitologiaStatus.LitologiaNãoEditada,
                Mensagem = $"Não foi possível não editar a litologia. {mensagem}"
            };
        }
    }
}
