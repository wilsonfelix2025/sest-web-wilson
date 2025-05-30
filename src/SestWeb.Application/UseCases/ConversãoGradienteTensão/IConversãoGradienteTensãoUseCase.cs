using System.Threading.Tasks;

namespace SestWeb.Application.UseCases.ConversãoGradienteTensão
{
    public interface IConversãoGradienteTensãoUseCase
    {
        Task<ConversãoOutput> Execute(ConversãoInput input);

    }
}
