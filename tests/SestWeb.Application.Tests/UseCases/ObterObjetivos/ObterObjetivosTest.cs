using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.ObjetivoUseCases.ObterObjetivos;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Poço.Objetivos;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.ObterObjetivos
{
    [TestFixture]
    public class ObterObjetivosTest
    {
        [Test]
        public async Task DeveRetornarObjetivosSeEncontrouPoço()
        {
            // Arrange
            const string nome = "Poço1";
            const TipoPoço tipo = TipoPoço.Projeto;

            var poço = PoçoFactory.CriarPoço(nome, nome, tipo);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterObjetivos(A<string>.Ignored)).Returns(poço.Objetivos);

            var obterObjetivosUseCase = new ObterObjetivosUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterObjetivosUseCase.Execute(poço.Id.ToString());

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterObjetivosStatus.ObjetivosObtidos);
            Check.That(result.Objetivos).IsNotNull();
            Check.That(result.Mensagem).IsEqualTo("Objetivos obtidos com sucesso.");
        }

        [Test]
        public async Task DeveRetornarPoçoNãoEncontradoSeNãoExistePoço()
        {
            // Arrange
            const string id = "id";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterObjetivos(A<string>.Ignored)).Returns(Task.FromResult<IReadOnlyCollection<Objetivo>>(null));

            var obterObjetivosUseCase = new ObterObjetivosUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterObjetivosUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterObjetivosStatus.PoçoNãoEncontrado);
            Check.That(result.Objetivos).IsNull();
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }

        [Test]
        public async Task DeveRetornarObjetivosNãoObtidasSeNãoConseguiuObterObjetivos()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterObjetivos(A<string>.Ignored)).ThrowsAsync(new Exception(mensagemDeErro));

            var obterObjetivosUseCase = new ObterObjetivosUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterObjetivosUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterObjetivosStatus.ObjetivosNãoObtidos);
            Check.That(result.Objetivos).IsNull();
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível obter objetivos. {mensagemDeErro}");
        }
    }
}