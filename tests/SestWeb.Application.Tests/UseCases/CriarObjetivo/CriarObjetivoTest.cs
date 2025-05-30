using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.ObjetivoUseCases.CriarObjetivo;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Poço.Objetivos;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.CriarObjetivo
{
    [TestFixture]
    public class CriarObjetivoTest
    {
        [Test]
        public async Task DeveCriarObjetivo()
        {
            // Arrange
            const string idPoço = "id";
            const string nomePoço = "nomePoço";
            const TipoPoço tipoPoço = TipoPoço.Retroanalise;
            const double pm = 1000.0;
            const TipoObjetivo tipoObjetivo = TipoObjetivo.Primário;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExisteObjetivoNaProfundidade(A<string>.Ignored, A<double>.Ignored)).Returns(false);
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(PoçoFactory.CriarPoço(idPoço, nomePoço, tipoPoço));
            A.CallTo(() => poçoWriteOnlyRepository.CriarObjetivo(A<string>.Ignored, A<Objetivo>.Ignored)).Returns(true);

            var criarObjetivoUseCase = new CriarObjetivoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);
            
            // Act
            var result = await criarObjetivoUseCase.Execute(idPoço, pm, tipoObjetivo);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarObjetivoStatus.ObjetivoCriado);
            Check.That(result.Mensagem).IsEqualTo("Objetivo criado com sucesso.");
        }

        [Test]
        public async Task NãoDeveCriarObjetivoSeJáExisteNaMesmaProfundidade()
        {
            // Arrange
            const string idPoço = "id";
            const double pm = 1000.0;
            const TipoObjetivo tipoObjetivo = TipoObjetivo.Primário;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExisteObjetivoNaProfundidade(A<string>.Ignored, A<double>.Ignored)).Returns(true);
            A.CallTo(() => poçoWriteOnlyRepository.CriarObjetivo(A<string>.Ignored, A<Objetivo>.Ignored)).Returns(false);

            var criarObjetivoUseCase = new CriarObjetivoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);


            // Act
            var result = await criarObjetivoUseCase.Execute(idPoço, pm, tipoObjetivo);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarObjetivoStatus.ObjetivoNãoCriado);
            Check.That(result.Mensagem).IsEqualTo($"Já existe objetivo na profundidade {pm}.");
        }

        [Test]
        public async Task NãoConseguiuCriarObjetivo()
        {
            // Arrange
            const string idPoço = "id";
            const string nomePoço = "nomePoço";
            const TipoPoço tipoPoço = TipoPoço.Retroanalise;
            const double pm = 1000.0;
            const TipoObjetivo tipoObjetivo = TipoObjetivo.Primário;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExisteObjetivoNaProfundidade(A<string>.Ignored, A<double>.Ignored)).Returns(false);
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(PoçoFactory.CriarPoço(idPoço, nomePoço, tipoPoço));
            A.CallTo(() => poçoWriteOnlyRepository.CriarObjetivo(A<string>.Ignored, A<Objetivo>.Ignored)).Returns(false);

            var criarObjetivoUseCase = new CriarObjetivoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);
            
            // Act
            var result = await criarObjetivoUseCase.Execute(idPoço, pm, tipoObjetivo);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarObjetivoStatus.ObjetivoNãoCriado);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível criar o objetivo. ");
        }

        [Test]
        public async Task NãoConseguiuCriarObjetivoPorqueLançouExceção()
        {
            // Arrange
            const string idPoço = "id";
            const string nomePoço = "nomePoço";
            const TipoPoço tipoPoço = TipoPoço.Retroanalise;
            const double pm = 1000.0;
            const TipoObjetivo tipoObjetivo = TipoObjetivo.Primário;
            const string mensagem = "Razão da exceção.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExisteObjetivoNaProfundidade(A<string>.Ignored, A<double>.Ignored)).Returns(false);
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(PoçoFactory.CriarPoço(idPoço, nomePoço, tipoPoço));
            A.CallTo(() => poçoWriteOnlyRepository.CriarObjetivo(A<string>.Ignored, A<Objetivo>.Ignored)).ThrowsAsync(new Exception(mensagem));

            var criarObjetivoUseCase = new CriarObjetivoUseCase(poçoReadOnlyRepository, poçoWriteOnlyRepository);
            
            // Act
            var result = await criarObjetivoUseCase.Execute(idPoço, pm, tipoObjetivo);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarObjetivoStatus.ObjetivoNãoCriado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível criar o objetivo. {mensagem}");
        }
    }
}