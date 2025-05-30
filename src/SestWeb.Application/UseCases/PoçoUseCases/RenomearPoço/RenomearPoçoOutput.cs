using SestWeb.Application.Helpers;

namespace SestWeb.Application.UseCases.PoçoUseCases.RenomearPoço
{
    public class RenomearPoçoOutput : UseCaseOutput<RenomearPoçoStatus>
    {
        private RenomearPoçoOutput()
        {
        }

        public static RenomearPoçoOutput PoçoRenomeado()
        {
            return new RenomearPoçoOutput
            {
                Status = RenomearPoçoStatus.PoçoRenomeado,
                Mensagem = "Poço renomeado com sucesso."
            };
        }

        public static RenomearPoçoOutput PoçoNãoEncontrado(string id)
        {
            return new RenomearPoçoOutput
            {
                Status = RenomearPoçoStatus.PoçoNãoEncontrado,
                Mensagem = $"Não foi possível encontrar poço com id {id}."
            };
        }

        public static RenomearPoçoOutput JáExistePoçoComEsseNome()
        {
            return new RenomearPoçoOutput
            {
                Status = RenomearPoçoStatus.PoçoNãoRenomeado,
                Mensagem = "Não foi possível criar poço. Já existe poço com esse nome."
            };
        }

        public static RenomearPoçoOutput PoçoNãoRenomeado(string mensagem = "")
        {
            return new RenomearPoçoOutput
            {
                Status = RenomearPoçoStatus.PoçoNãoRenomeado,
                Mensagem = $"Não foi possível renomear o poço. {mensagem}"
            };
        }
    }
}
