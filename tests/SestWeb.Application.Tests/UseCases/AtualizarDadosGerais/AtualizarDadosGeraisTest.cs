using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarDadosGerais;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.AtualizarDadosGerais
{
    [TestFixture]
    public class AtualizarDadosGeraisTest
    {
        [Test]
        public async Task DeveRetornarPoçoNãoEncontradoSeNãoEncontrouPoço()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            var dadosGeraisInput = new AtualizarDadosGeraisInput();

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult<Poço>(null));

            var atualizarDadosGeraisUseCase = new AtualizarDadosGeraisUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await atualizarDadosGeraisUseCase.Execute(id, dadosGeraisInput);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(AtualizarDadosGeraisStatus.PoçoNãoEncontrado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }

        [Test]
        public async Task DeveRetornarDadosGeraisAtualizadoSeFoiAlteradoComSucesso()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var poço = PoçoFactory.CriarPoço("Poço", "Poço", TipoPoço.Projeto);

            var dadosGeraisInput = new AtualizarDadosGeraisInput();
            dadosGeraisInput.Identificação.Nome = "teste";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(id)).Returns(poço);
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarDadosGerais(id, A<Poço>.Ignored, false)).Returns(true);

            var atualizarDadosGeraisUseCase = new AtualizarDadosGeraisUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await atualizarDadosGeraisUseCase.Execute(id, dadosGeraisInput);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(AtualizarDadosGeraisStatus.DadosGeraisAtualizados);
            Check.That(result.Mensagem).IsEqualTo("Dados gerais atualizados com sucesso.");
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarDadosGerais(A<string>.Ignored, A<Poço>.Ignored, false)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task DeveRetornarDadosGeraisNãoAtualizadosSeNãoConseguiuAtualizar()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var poço = PoçoFactory.CriarPoço("Poço", "Poço", TipoPoço.Projeto);

            var dadosGeraisInput = new AtualizarDadosGeraisInput();

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(id)).Returns(poço);
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarDadosGerais(id, A<Poço>.Ignored, false)).Returns(false);

            var atualizarDadosGeraisUseCase = new AtualizarDadosGeraisUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await atualizarDadosGeraisUseCase.Execute(id, dadosGeraisInput);

            // Assert
            Check.That(result.Status).IsEqualTo(AtualizarDadosGeraisStatus.DadosGeraisNãoAtualizados);
            Check.That(result.Mensagem).Contains($"Não foi possível atualizar dados gerais.");
        }

        [Test]
        public async Task DeveRetornarDadosGeraisNãoAtualizadosSeHouveExceção()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var poço = PoçoFactory.CriarPoço("Poço", "Poço", TipoPoço.Projeto);

            var dadosGeraisInput = new AtualizarDadosGeraisInput();
            dadosGeraisInput.Identificação.Nome = "teste";

            const string mensagemErro = "Mensagem da exceção.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(id)).Returns(poço);
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarDadosGerais(A<string>.Ignored, A<Poço>.Ignored, false)).ThrowsAsync(new Exception(mensagemErro));

            var atualizarDadosGeraisUseCase = new AtualizarDadosGeraisUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await atualizarDadosGeraisUseCase.Execute(id, dadosGeraisInput);

            // Assert
            Check.That(result.Status).IsEqualTo(AtualizarDadosGeraisStatus.DadosGeraisNãoAtualizados);
            Check.That(result.Mensagem).Contains($"Não foi possível atualizar dados gerais. {mensagemErro}");
        }
    }
}