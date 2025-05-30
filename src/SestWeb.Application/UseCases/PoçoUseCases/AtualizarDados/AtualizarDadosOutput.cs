using SestWeb.Application.Helpers;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using System.Collections.Generic;

namespace SestWeb.Application.UseCases.PoçoUseCases.AtualizarDados
{
    public class AtualizarDadosOutput : UseCaseOutput<AtualizarDadosStatus>
    {
        public AtualizarDadosOutput()
        {
                
        }

        public List<PerfilBase> PerfisAlterados { get; set; }
        public List<Litologia> Litologias { get; set; }

        public static AtualizarDadosOutput DadosAtualizados(List<PerfilBase> perfisAlterados, List<Litologia> litologias)
        {
            return new AtualizarDadosOutput
            {
                Status = AtualizarDadosStatus.DadosAtualizados,
                Mensagem = "Dados atualizados com sucesso.",
                PerfisAlterados = perfisAlterados,
                Litologias = litologias
            };
        }

        public static AtualizarDadosOutput PoçoNãoEncontrado(string id)
        {
            return new AtualizarDadosOutput
            {
                Status = AtualizarDadosStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }

        public static AtualizarDadosOutput DadosNãoAtualizados(string mensagem = "")
        {
            return new AtualizarDadosOutput
            {
                Status = AtualizarDadosStatus.DadosNãoAtualizados,
                Mensagem = $"Não foi possível atualizar dados. {mensagem}"
            };
        }
    }
}
