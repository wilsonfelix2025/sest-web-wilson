using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PoçoUseCases.AtualizarState;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.AtualizarState
{
    [TestFixture]
    public class AtualizarStateTests
    {
        [Test]
        public async Task DeveRetornarDadosGeraisAtualizadoSeFoiAlteradoComSucesso()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var poço = PoçoFactory.CriarPoço("Poço", "Poço", TipoPoço.Retroanalise);

            var atualizarStateInput = new AtualizarStateInput();
            atualizarStateInput.Tree.Add(new TreeInput { Id = "0", Name = "Trajetória", Fixa = true });
            atualizarStateInput.Tree.Add(new TreeInput
            {
                Id = "1",
                Name = "Perfis",
                Fixa = true,
                Data = new List<TreeInput>
                {
                    new TreeInput {Id = "2", Name = "DTC", Fixa = true},
                    new TreeInput {Id = "3", Name = "DTS", Fixa = true}
                }
            });
            atualizarStateInput.Tree.Add(new TreeInput { Id = "4", Name = "Tensões/Pressões", Fixa = true, Data = new List<TreeInput>() });
            atualizarStateInput.Tree.Add(new TreeInput { Id = "5", Name = "Gradientes", Fixa = true, Data = new List<TreeInput>() });

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(id)).Returns(poço);
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarPoço(A<Poço>.Ignored)).Returns(true);

            var atualizarStateUseCase = new AtualizarStateUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await atualizarStateUseCase.Execute(id, atualizarStateInput);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(AtualizarStateStatus.StateAtualizado);
            Check.That(result.Mensagem).IsEqualTo("State atualizado com sucesso.");
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarPoço(A<Poço>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task DeveRetornarPoçoNãoEncontradoSeNãoEncontrouPoço()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            var atualizarStateInput = new AtualizarStateInput();

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult<Poço>(null));

            var atualizarStateUseCase = new AtualizarStateUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await atualizarStateUseCase.Execute(id, atualizarStateInput);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(AtualizarStateStatus.PoçoNãoEncontrado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível encontrar poço com id {id}.");
        }
    }
}