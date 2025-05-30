using System.Collections.Generic;
using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Application.UseCases.MontagemPerfis
{
    public class MontarPerfisOutput : UseCaseOutput<MontarPerfisStatus>
    {
        public MontarPerfisOutput()
        {
            
        }


        public List<PerfilBase> ListaPerfis = new List<PerfilBase>();
        public Litologia Litologia;

        public static MontarPerfisOutput MontarPerfisComSucesso(List<PerfilBase> listaPerfis, Litologia litologia)
        {
            return new MontarPerfisOutput
            {
                Status = MontarPerfisStatus.MontarPerfisComSucesso,
                Mensagem = "Montagem de perfis com sucesso.",
                ListaPerfis = listaPerfis,
                Litologia = litologia,
            };
        }

        public static MontarPerfisOutput MontarPerfisComFalhaDeValidação(string mensagem)
        {
            return new MontarPerfisOutput
            {
                Status = MontarPerfisStatus.MontarPerfisComFalhaDeValidação,
                Mensagem = $"Não foi possível montar perfis. {mensagem}"
            };
        }

        public static MontarPerfisOutput MontarPerfisComFalha(string mensagem)
        {
            return new MontarPerfisOutput
            {
                Status = MontarPerfisStatus.MontarPerfisComFalha,
                Mensagem = $"Não foi possível montar perfis. {mensagem}"
            };
        }

    }
}
