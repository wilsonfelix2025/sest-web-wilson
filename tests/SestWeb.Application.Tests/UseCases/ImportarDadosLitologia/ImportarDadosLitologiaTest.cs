using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarDadosUseCase.ImportarLitologiaUseCase;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.ImportarDadosLitologia
{
    [TestFixture]
    public class ImportarDadosLitologiaTest
    {
        [Test]
        public async Task DeveRetornarErroDeValidaçãoSeNãoEncontrouPoço()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var input = new LitologiaInput();

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult<Poço>(null));

            var useCase = new ImportarDadosLitologiaUseCase(poçoWriteOnlyRepository, poçoReadOnlyRepository, perfilWriteOnlyRepository);

            // Act
            var result = await useCase.Execute(id, input);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ImportarDadosStatus.ImportaçãoComFalhasDeValidação);
            Check.That(result.Mensagem).IsEqualTo($"Importação não realizada dos dados. Foram encontrados erros de validação. Não foi possível encontrar poço com id {id}");
        }

        [Test]
        public async Task DeveRetornarErroDeValidaçãoSeNãoFoiFornecidoDadosDeImportação()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();

            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult<Poço>(null));

            var useCase = new ImportarDadosLitologiaUseCase(poçoWriteOnlyRepository, poçoReadOnlyRepository, perfilWriteOnlyRepository);

            // Act
            var result = await useCase.Execute(id, null);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ImportarDadosStatus.ImportaçãoComFalhasDeValidação);
            Check.That(result.Mensagem).IsEqualTo("Importação não realizada dos dados. Foram encontrados erros de validação. Dados para importação de planilha não preenchidos");
        }

        
        [Test]
        public async Task DeveRetornarSucesso()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var poço = PoçoFactory.CriarPoço(id, "poçoTeste", TipoPoço.Projeto);

            poço.DadosGerais.Geometria.OffShore.LaminaDagua = 10;
            poço.DadosGerais.Geometria.MesaRotativa = 10;

            var input = new LitologiaInput
            {
                TipoLitologia = "Prevista"
            };

            input.PontosLitologia.Add(new PontoLitologiaInput
            {
                PM = 1000,
                TipoRocha = "CAL"
            });


            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();

            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult(poço));
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarLitologias(A<string>.Ignored, A<Poço>.Ignored)).Returns(true);

            var useCase = new ImportarDadosLitologiaUseCase(poçoWriteOnlyRepository, poçoReadOnlyRepository, perfilWriteOnlyRepository);
            
            // Act
            var result = await useCase.Execute(id, input);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ImportarDadosStatus.ImportadoComSucesso);
        }

    }
}
