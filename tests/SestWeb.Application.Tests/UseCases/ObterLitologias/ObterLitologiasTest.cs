using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.LitologiaUseCases.ObterLitologias;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.ObterLitologias
{
    [TestFixture]
    public class ObterLitologiasTest
    {
        [Test]
        public async Task DeveRetornarLitologiasSeEncontrouPoço()
        {
            // Arrange
            const string nome = "Poço1";
            const TipoPoço tipo = TipoPoço.Projeto;

            var poço = PoçoFactory.CriarPoço(nome, nome, tipo);
            poço.Litologias.Add(new Litologia(TipoLitologia.Prevista, poço.Trajetória));
            poço.Litologias.Add(new Litologia(TipoLitologia.Adaptada, poço.Trajetória));

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoReadOnlyRepository.ObterLitologias(A<string>.Ignored)).Returns(poço.Litologias);

            var obterLitologiasUseCase = new ObterLitologiasUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterLitologiasUseCase.Execute(poço.Id.ToString());

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterLitologiasStatus.LitologiasObtidas);
            Check.That(result.Litologias).IsNotNull();
            Check.That(result.Litologias).CountIs(poço.Litologias.Count);
            Check.That(result.Mensagem).IsEqualTo("Litologias obtidas com sucesso.");
        }

        [Test]
        public async Task DeveRetornarPoçoNãoEncontradoSeNãoExistePoço()
        {
            // Arrange
            const string id = "id";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(false);

            var obterLitologiasUseCase = new ObterLitologiasUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterLitologiasUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterLitologiasStatus.PoçoNãoEncontrado);
            Check.That(result.Litologias).IsNull();
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }

        [Test]
        public async Task DeveRetornarLitologiasNãoObtidasSeNãoConseguiuObterLitologias()
        {
            // Arrange
            const string id = "id";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoReadOnlyRepository.ObterLitologias(A<string>.Ignored)).Returns(Task.FromResult<IReadOnlyCollection<Litologia>>(null));

            var obterLitologiasUseCase = new ObterLitologiasUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterLitologiasUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterLitologiasStatus.LitologiasNãoObtidas);
            Check.That(result.Litologias).IsNull();
            Check.That(result.Mensagem).StartsWith("Litologias não obtidas.");
        }

        [Test]
        public async Task DeveRetornarLitologiasNãoObtidasSeLançouExceção()
        {
            // Arrange
            const string id = "id";
            const string mensagemDeErro = "Mensagem de erro.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoReadOnlyRepository.ObterLitologias(A<string>.Ignored)).ThrowsAsync(new Exception(mensagemDeErro));

            var obterLitologiasUseCase = new ObterLitologiasUseCase(poçoReadOnlyRepository);

            // Act
            var result = await obterLitologiasUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterLitologiasStatus.LitologiasNãoObtidas);
            Check.That(result.Litologias).IsNull();
            Check.That(result.Mensagem).IsEqualTo($"Litologias não obtidas. {mensagemDeErro}");
        }
    }
}