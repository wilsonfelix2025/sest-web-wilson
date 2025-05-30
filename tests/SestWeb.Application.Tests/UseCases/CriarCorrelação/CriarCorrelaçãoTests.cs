using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.Tests.Helpers;
using SestWeb.Application.UseCases.CorrelaçãoUseCases.CriarCorrelação;
using SestWeb.Domain;
using SestWeb.Domain.Entities.Correlações.Base.Factory;

namespace SestWeb.Application.Tests.UseCases.CriarCorrelação
{
    public class CriarCorrelaçãoTests : IoCSupportedTest<DomainModule>
    {
        private ICorrelaçãoFactory _correlaçãoFactory;

        [OneTimeSetUp]
        public void Setup()
        {
            _correlaçãoFactory = Resolve<ICorrelaçãoFactory>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _correlaçãoFactory = null;
            ShutdownIoC();
        }

        [Test]
        public async Task DeveCriarCorrelaçãoCorretamenteParaDadosVálidos()
        {
            // Arrange
            string idPoço = "0";
            string nome = "correlaçãoTeste";
            string nomeAutor = "autorTeste";
            string chaveAutor = "chaveTeste";
            string descrição = "descriçãoteste";
            string expressão = "DTS = DTC + DTMC";

            var correlaçãoWriteOnlyRepository = A.Fake<ICorrelaçãoWriteOnlyRepository>();
            var correlaçãoReadOnlyRepository = A.Fake<ICorrelaçãoReadOnlyRepository>();
            var correlaçãoPoçoReadOnlyRepository = A.Fake<ICorrelaçãoPoçoReadOnlyRepository>();
            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => correlaçãoReadOnlyRepository.ExisteCorrelação(A<string>.Ignored)).Returns(false);
            A.CallTo(() => correlaçãoPoçoReadOnlyRepository.ExisteCorrelaçãoPoço(A<string>.Ignored, A<string>.Ignored)).Returns(false);

            var criarCorrelaçãoUseCase = new CriarCorrelaçãoUseCase(correlaçãoWriteOnlyRepository,
                correlaçãoReadOnlyRepository, correlaçãoPoçoReadOnlyRepository, _correlaçãoFactory, poçoReadOnlyRepository);

            // Act
            var result = await criarCorrelaçãoUseCase.Execute(idPoço, nome, nomeAutor,chaveAutor,descrição,expressão);

#if DEBUG
            if (result.Status == CriarCorrelaçãoStatus.CorrelaçãoNãoCriada)
                Console.WriteLine($"*** {result.Mensagem} ***");
#endif
            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarCorrelaçãoStatus.CorrelaçãoCriada);
            Check.That(result.Mensagem).IsEqualTo("Correlação criada com sucesso.");
        }

        [Test]
        public async Task NãoDeveCriarCorrelaçãoSeJáExisteCorrelaçãoCoMesmoNome()
        {
            // Arrange
            string idPoço = "0";
            string nome = "correlaçãoTeste";
            string nomeAutor = "autorTeste";
            string chaveAutor = "chaveTeste";
            string descrição = "descriçãoteste";
            string origem = "Usuário";
            string expressão = "DTS = DTC + DTMC";

            var correlaçãoWriteOnlyRepository = A.Fake<ICorrelaçãoWriteOnlyRepository>();
            var correlaçãoReadOnlyRepository = A.Fake<ICorrelaçãoReadOnlyRepository>();
            var correlaçãoPoçoReadOnlyRepository = A.Fake<ICorrelaçãoPoçoReadOnlyRepository>();
            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => correlaçãoReadOnlyRepository.ExisteCorrelação(A<string>.Ignored)).Returns(true);
            A.CallTo(() => correlaçãoPoçoReadOnlyRepository.ExisteCorrelaçãoPoço(A<string>.Ignored, A<string>.Ignored))
                .Returns(false);

            var criarCorrelaçãoUseCase = new CriarCorrelaçãoUseCase(correlaçãoWriteOnlyRepository,
                correlaçãoReadOnlyRepository, correlaçãoPoçoReadOnlyRepository,_correlaçãoFactory, poçoReadOnlyRepository);

            // Act
            var result = await criarCorrelaçãoUseCase.Execute(idPoço, nome, nomeAutor, chaveAutor, descrição, expressão);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarCorrelaçãoStatus.CorrelaçãoNãoCriada);
            Check.That(result.Mensagem).IsEqualTo("Já existe uma correlação com esse nome: correlaçãoTeste");
        }

        [Test]
        public async Task NãoDeveCriarCorrelaçãoComDadosInválidos()
        {
            // Arrange
            string idPoço = "0";
            string nome = "correlaçãoTeste";
            string nomeAutor = "autorTeste";
            string chaveAutor = "chaveTeste";
            string descrição = "descriçãoteste";
            string expressão = "var DTC = 1, DTS = DTC + DTMC";

            var correlaçãoWriteOnlyRepository = A.Fake<ICorrelaçãoWriteOnlyRepository>();
            var correlaçãoReadOnlyRepository = A.Fake<ICorrelaçãoReadOnlyRepository>();
            var correlaçãoPoçoReadOnlyRepository = A.Fake<ICorrelaçãoPoçoReadOnlyRepository>();
            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => correlaçãoReadOnlyRepository.ExisteCorrelação(A<string>.Ignored)).Returns(false);
            A.CallTo(() => correlaçãoPoçoReadOnlyRepository.ExisteCorrelaçãoPoço(A<string>.Ignored, A<string>.Ignored))
                .Returns(false);

            var criarCorrelaçãoUseCase = new CriarCorrelaçãoUseCase(correlaçãoWriteOnlyRepository,
                correlaçãoReadOnlyRepository, correlaçãoPoçoReadOnlyRepository, _correlaçãoFactory, poçoReadOnlyRepository);

            // Act
            var result = await criarCorrelaçãoUseCase.Execute(idPoço, nome, nomeAutor, chaveAutor, descrição, expressão);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarCorrelaçãoStatus.CorrelaçãoNãoCriada);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível criar correlação: Variável \"DTC\" inválida - Nome reservado para entrada de perfis!");
        }
    }
}
