using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.PoçoUseCases.CriarPoço
{
    public class CriarPoçoOutput : UseCaseOutput<CriarPoçoStatus>
    {
        private CriarPoçoOutput()
        {
        }

        public PoçoOutput Poço { get; private set; }

        public static CriarPoçoOutput PoçoCriado(PoçoOutput poço)
        {
            return new CriarPoçoOutput
            {
                Status = CriarPoçoStatus.PoçoCriado,
                Mensagem = "Poço criado com sucesso.",
                Poço = poço
            };
        }

        public static CriarPoçoOutput JáExistePoçoComEsseNome()
        {
            return new CriarPoçoOutput
            {
                Status = CriarPoçoStatus.PoçoNãoCriado,
                Mensagem = "Não foi possível criar poço. Já existe poço com esse nome."
            };
        }

        public static CriarPoçoOutput PoçoNãoCriado(string mensagem = "")
        {
            return new CriarPoçoOutput
            {
                Status = CriarPoçoStatus.PoçoNãoCriado,
                Mensagem = $"Não foi possível criar poço. {mensagem}"
            };
        }
    }
}