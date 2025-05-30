using System.Threading.Tasks;
using NUnit.Framework;

namespace SestWeb.Application.Tests.UseCases.ObterPoço
{
    [TestFixture]
    public class ObterPoçoTest
    {
        [Test]
        public async Task DeveRetornarPoçoSeEncontrouPoço()
        {
            // Arrange
            // const string nome = "Poço1";
            // const TipoPoço tipo = TipoPoço.Projeto;

            // var poço = PoçoFactory.CriarPoço(nome, nome, tipo);

            // var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            // A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult(poço));

            // var obterPoçoUseCase = new ObterPoçoUseCase(poçoReadOnlyRepository);

            // // Act
            // var result = await obterPoçoUseCase.Execute("id");

            // // Assert
            // Check.That(result).IsNotNull();
            // Check.That(result.Status).IsEqualTo(ObterPoçoStatus.PoçoObtido);
            // Check.That(result.Poço).IsNotNull();
            // Check.That(result.Poço.Nome).IsEqualTo(nome);
            // Check.That(result.Poço.TipoPoço).IsEqualTo(tipo);
        }

        [Test]
        public async Task DeveRetornarNotFoundSeNãoExistePoço()
        {
            // Arrange
            // var id = Guid.NewGuid().ToString();

            // var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            // A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult<Poço>(null));

            // var obterPoçoUseCase = new ObterPoçoUseCase(poçoReadOnlyRepository);

            // // Act
            // var result = await obterPoçoUseCase.Execute(id);

            // // Assert
            // Check.That(result).IsNotNull();
            // Check.That(result.Status).IsEqualTo(ObterPoçoStatus.PoçoNãoEncontrado);
            // Check.That(result.Poço).IsNull();
            // Check.That(result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }

        [Test]
        public async Task DeveRetornarBadRequestSeHouveErro()
        {
            // Arrange
            // var id = Guid.NewGuid().ToString();
            // var mensagemErro = "Mensagem de erro";

            // var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            // A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).ThrowsAsync(new Exception(mensagemErro));

            // var obterPoçoUseCase = new ObterPoçoUseCase(poçoReadOnlyRepository);

            // // Act
            // var result = await obterPoçoUseCase.Execute(id);

            // // Assert
            // Check.That(result.Status).IsEqualTo(ObterPoçoStatus.PoçoNãoObtido);
            // Check.That(result.Poço).IsNull();
            // Check.That(result.Mensagem).Contains($"Não foi possível obter poço. {mensagemErro}");
        }
    }
}