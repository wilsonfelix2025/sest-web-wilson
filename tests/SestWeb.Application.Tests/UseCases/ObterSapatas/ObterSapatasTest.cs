using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.SapataUseCases.ObterSapatas;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Poço.Sapatas;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.ObterSapatas
{
    [TestFixture]
    public class ObterSapatasTest
    {
        [Test]
        public async Task DeveRetornarSapatasSeEncontrouPoço()
        {
            // Arrange
            const string nome = "Poço1";
            const TipoPoço tipo = TipoPoço.Projeto;

            var poço = PoçoFactory.CriarPoço(nome, nome, tipo);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterSapatas(A<string>.Ignored)).Returns(poço.Sapatas);

            var obterSapatasUseCase = new ObterSapatasUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterSapatasUseCase.Execute(poço.Id.ToString());

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterSapatasStatus.SapatasObtidas);
            Check.That(result.Sapatas).IsNotNull();
            Check.That(result.Mensagem).IsEqualTo("Sapatas obtidas com sucesso.");
        }

        [Test]
        public async Task DeveRetornarPoçoNãoEncontradoSeNãoExistePoço()
        {
            // Arrange
            const string id = "id";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterSapatas(A<string>.Ignored)).Returns(Task.FromResult<IReadOnlyCollection<Sapata>>(null));

            var obterSapatasUseCase = new ObterSapatasUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterSapatasUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterSapatasStatus.PoçoNãoEncontrado);
            Check.That(result.Sapatas).IsNull();
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }

        [Test]
        public async Task DeveRetornarSapatasNãoObtidasSeNãoConseguiuObterSapatas()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterSapatas(A<string>.Ignored)).ThrowsAsync(new Exception(mensagemDeErro));

            var obterSapatasUseCase = new ObterSapatasUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterSapatasUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterSapatasStatus.SapatasNãoObtidas);
            Check.That(result.Sapatas).IsNull();
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível obter sapatas. {mensagemDeErro}");
        }
    }
}