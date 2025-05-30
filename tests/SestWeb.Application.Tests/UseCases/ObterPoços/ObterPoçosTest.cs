using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PoçoUseCases.ObterPoços;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.ObterPoços
{
    [TestFixture]
    public class ObterPoçosTest
    {
        [Test]
        public async Task DeveRetornarPoçosObtidosSeEncontrouPoços()
        {
            // Arrange
            const string nome = "Poço1";
            const TipoPoço tipo = TipoPoço.Projeto;

            var poço = PoçoFactory.CriarPoço(nome, nome, tipo);
            var poços = new List<Poço> { poço };

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoços()).Returns(poços);

            var obterPoçoUseCase = new ObterPoçosUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterPoçoUseCase.Execute();

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterPoçosStatus.PoçosObtidos);
            Check.That(result.Poços).IsNotNull();
            Check.That(result.Poços).CountIs(1);
            Check.That(result.Status).IsEqualTo(ObterPoçosStatus.PoçosObtidos);
            Check.That(result.Mensagem).IsEqualTo("Poços obtidos com sucesso.");
        }
    }
}