using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PoçoUseCases.ObterDadosGerais;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.ObterDadosGerais
{
    [TestFixture]
    public class ObterDadosGeraisTest
    {
        [Test]
        public async Task DeveRetornarDadosGeraisObtidosSeEncontrouPoço()
        {
            // Arrange
            const string nome = "Poço1";
            const TipoPoço tipo = TipoPoço.Projeto;

            var poço = PoçoFactory.CriarPoço(nome, nome, tipo);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterDadosGerais(A<string>.Ignored)).Returns(Task.FromResult(poço.DadosGerais));

            var obterDadosGeraisUseCase = new ObterDadosGeraisUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterDadosGeraisUseCase.Execute("id");

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterDadosGeraisStatus.DadosGeraisObtidos);
            Check.That(result.DadosGerais).IsNotNull();
            Check.That(result.Mensagem).IsEqualTo("Dados gerais obtidos com sucesso.");
            Check.That(result.DadosGerais.Identificação.Nome).IsEqualTo(nome);
            Check.That(result.DadosGerais.Identificação.TipoPoço).IsEqualTo(tipo);
        }

        [Test]
        public async Task DeveRetornarPoçoNãoEncontradoSeNãoExistePoço()
        {
            // Arrange
            const string id = "id";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterDadosGerais(A<string>.Ignored)).Returns(Task.FromResult<DadosGerais>(null));

            var obterDadosGeraisUseCase = new ObterDadosGeraisUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterDadosGeraisUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterDadosGeraisStatus.PoçoNãoEncontrado);
            Check.That(result.DadosGerais).IsNull();
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }

        [Test]
        public async Task DeveRetornarDadosGeraisNãoObtidosSeHouveExceção()
        {
            // Arrange
            const string mensagemErro = "Mensagem da exceção.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterDadosGerais(A<string>.Ignored)).ThrowsAsync(new Exception(mensagemErro));

            var obterDadosGeraisUseCase = new ObterDadosGeraisUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterDadosGeraisUseCase.Execute(string.Empty);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterDadosGeraisStatus.DadosGeraisNãoObtidos);
            Check.That(result.DadosGerais).IsNull();
            Check.That(result.Mensagem).Contains($"Não foi possível obter dados gerais. {mensagemErro}");
        }
    }
}