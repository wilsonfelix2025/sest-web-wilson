using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.LitologiaUseCases.RemoverLitologia;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.RemoverLitologia
{
    [TestFixture]
    public class RemoverLitologiaTest
    {
        [Test]
        public async Task DeveRetornarLitologiaRemovidaSeRemoverLitologia()
        {
            // Arrange
            const string id = "id";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            var poço = PoçoFactory.CriarPoço(id, "Teste", TipoPoço.Projeto);
            var litologia = new Litologia(TipoLitologia.Prevista, poço.Trajetória);
            poço.Litologias.Add(litologia);
            var idLitologia = litologia.Id.ToString();

            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(poço);
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarPoço(poço)).Returns(true);

            var removerLitologiaUseCase = new RemoverLitologiaUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerLitologiaUseCase.Execute(id, idLitologia);

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverLitologiaStatus.LitologiaRemovida);
            Check.That(result.Mensagem).IsEqualTo("Litologia removida com sucesso.");
        }

        [Test]
        public void DeveRetornarPoçoNãoEncontradoSePoçoNãoExiste()
        {
            // Arrange
            const string id = "id";
            const string idLitologia = "idLitologia";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(false);

            var removerLitologiaUseCase = new RemoverLitologiaUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = removerLitologiaUseCase.Execute(id, idLitologia);

            // Assert
            Check.That(result.Result.Status).IsEqualTo(RemoverLitologiaStatus.PoçoNãoEncontrado);
            Check.That(result.Result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
            A.CallTo(() => poçoWriteOnlyRepository.RemoverPoço(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public async Task DeveRetornarLitologiaNãoRemovidaSeNãoConseguirRemoverLitologia()
        {
            // Arrange
            const string id = "id";
            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            var poço = PoçoFactory.CriarPoço(id, "Teste", TipoPoço.Projeto);
            var litologia = new Litologia(TipoLitologia.Prevista, poço.Trajetória);
            poço.Litologias.Add(litologia);
            var idLitologia = litologia.Id.ToString();

            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(poço);
            A.CallTo(() => poçoWriteOnlyRepository.RemoverObjetivo(A<string>.Ignored, A<double>.Ignored)).Returns(false);

            var removerLitologiaUseCase = new RemoverLitologiaUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerLitologiaUseCase.Execute(id, idLitologia);

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverLitologiaStatus.LitologiaNãoRemovida);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível remover a litologia. ");
        }

        [Test]
        public async Task DeveRetornarLitologiaNãoRemovidaSeRepositórioLançarException()
        {
            // Arrange
            const string id = "id";
            const string idLitologia = "idLitologia";
            const string mensagemDeErro = "Mensagem de erro.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).ThrowsAsync(new Exception(mensagemDeErro));

            var removerLitologiaUseCase = new RemoverLitologiaUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await removerLitologiaUseCase.Execute(id, idLitologia);

            // Assert
            Check.That(result.Status).IsEqualTo(RemoverLitologiaStatus.LitologiaNãoRemovida);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível remover a litologia. {mensagemDeErro}");
        }
    }
}