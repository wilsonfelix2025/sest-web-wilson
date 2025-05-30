using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.SapataUseCases.CriarSapata;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Poço.Sapatas;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.CriarSapata
{
    [TestFixture]
    public class CriarSapataTest
    {
        [Test]
        public async Task DeveCriarSapata()
        {
            // Arrange
            const string idPoço = "id";
            const double pm = 1000.0;
            const string diâmetro = "12 5/8";
            const string nomePoço = "nomePoço";
            const TipoPoço tipoPoço = TipoPoço.Retroanalise;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExisteSapataNaProfundidade(A<string>.Ignored, A<double>.Ignored)).Returns(false);
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(PoçoFactory.CriarPoço(idPoço, nomePoço, tipoPoço));
            A.CallTo(() => poçoWriteOnlyRepository.CriarSapata(A<string>.Ignored, A<Sapata>.Ignored)).Returns(true);

            var criarSapataUseCase = new CriarSapataUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);
            
            // Act
            var result = await criarSapataUseCase.Execute(idPoço, pm, diâmetro);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarSapataStatus.SapataCriada);
            Check.That(result.Mensagem).IsEqualTo("Sapata criada com sucesso.");
        }

        [Test]
        public async Task NãoDeveCriarSapataSeJáExisteNaMesmaProfundidade()
        {
            // Arrange
            const string idPoço = "id";
            const double pm = 1000.0;
            const string diâmetro = "12 5/8";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExisteSapataNaProfundidade(A<string>.Ignored, A<double>.Ignored)).Returns(true);
            A.CallTo(() => poçoWriteOnlyRepository.CriarSapata(A<string>.Ignored, A<Sapata>.Ignored)).Returns(false);

            var criarSapataUseCase = new CriarSapataUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await criarSapataUseCase.Execute(idPoço, pm, diâmetro);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarSapataStatus.SapataNãoCriada);
            Check.That(result.Mensagem).IsEqualTo($"Já existe sapata na profundidade {pm}.");
        }

        [Test]
        public async Task NãoConseguiuCriarSapata()
        {
            // Arrange
            const string idPoço = "id";
            const double pm = 1000.0;
            const string diâmetro = "12 5/8";
            const string nomePoço = "nomePoço";
            const TipoPoço tipoPoço = TipoPoço.Retroanalise;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExisteSapataNaProfundidade(A<string>.Ignored, A<double>.Ignored)).Returns(false);
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(PoçoFactory.CriarPoço(idPoço, nomePoço, tipoPoço));
            A.CallTo(() => poçoWriteOnlyRepository.CriarSapata(A<string>.Ignored, A<Sapata>.Ignored)).Returns(false);

            var criarSapataUseCase = new CriarSapataUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);

            // Act
            var result = await criarSapataUseCase.Execute(idPoço, pm, diâmetro);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarSapataStatus.SapataNãoCriada);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível criar a sapata.");
        }

        [Test]
        public async Task NãoConseguiuCriarSapataPorqueLançouExceção()
        {
            // Arrange
            const string idPoço = "id";
            const double pm = 1000.0;
            const string diâmetro = "12 5/8";
            const string nomePoço = "nomePoço";
            const TipoPoço tipoPoço = TipoPoço.Retroanalise;
            const string mensagem = "Razão da exceção.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExisteSapataNaProfundidade(A<string>.Ignored, A<double>.Ignored)).Returns(false);
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(PoçoFactory.CriarPoço(idPoço, nomePoço, tipoPoço));
            A.CallTo(() => poçoWriteOnlyRepository.CriarSapata(A<string>.Ignored, A<Sapata>.Ignored)).ThrowsAsync(new Exception(mensagem));

            var criarSapataUseCase = new CriarSapataUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);
            
            // Act
            var result = await criarSapataUseCase.Execute(idPoço, pm, diâmetro);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarSapataStatus.SapataNãoCriada);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível criar a sapata. {mensagem}");
        }
    }
}