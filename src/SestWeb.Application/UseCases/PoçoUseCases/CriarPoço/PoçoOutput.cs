using SestWeb.Domain.Entities.DadosGeraisDoPoco;

namespace SestWeb.Application.UseCases.PoçoUseCases.CriarPoço
{
    public class PoçoOutput
    {
        public string Id { get; }
        public string Nome { get; }
        public TipoPoço TipoPoço { get; }

        public PoçoOutput(string id, string nome, TipoPoço tipoPoço)
        {
            Id = id;
            Nome = nome;
            TipoPoço = tipoPoço;
        }
    }
}
