using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.Services;
using SestWeb.Application.UseCases.ImportaçãoUseCase.ImportarArquivoUseCase;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.ImportarArquivoPoçoWeb
{
    [TestFixture]
    public class ImportarArquivoPoçoWebTest
    {

        [Test]
        public async Task DeveRetornarErroSeNãoEncontrouPoço()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var dadosSelecionado = A.Fake<List<DadosSelecionadosEnum>>();
            var dadosExtras = A.Fake<Dictionary<string, object>>();
            var listaLitologia = A.Fake<List<LitologiaModel>>();
            var listaPerfis = A.Fake<List<PerfilModel>>();


            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            var registroReadOnlyRepository = A.Fake<IRegistrosEventosReadOnlyRepository>();
            var registroWriteOnlyRepository = A.Fake<IRegistrosEventosWriteOnlyRepository>();
            var poçoWebService = A.Fake<IPocoWebService>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult<Poço>(null));

            var useCase = new ImportarArquivoUseCase(poçoWriteOnlyRepository, poçoReadOnlyRepository, poçoWebService, registroReadOnlyRepository, registroWriteOnlyRepository);

            // Act
            var result = await useCase.Execute(dadosSelecionado, "caminho", listaPerfis, listaLitologia, id, "", dadosExtras);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ImportarArquivoStatus.ImportaçãoComFalha);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível importar o arquivo. Erro ao importar os dados. Poço não encontrado.");
        }

        [Test]
        public async Task DeveRetornarErroDeValidação()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var poço = PoçoFactory.CriarPoço(id, "poçoTeste", TipoPoço.Projeto);
            string json = "{\r\n    \"url\": \"https://pocoweb.petro.intelie.net/api/public/file/1819/?rev=7573\", \r\n    \"name\": \"caso mateus_roger\", \r\n    \"file_type\": \"dadosbasicos.drilling\", \r\n    \"description\": \"teste score\", \r\n    \"archived\": false, \r\n    \"created_at\": \"2018-08-14T14:56:42.100Z\", \r\n    \"last_updated_at\": \"2018-08-14T14:56:42.110Z\", \r\n    \"last_updated_by\": \"ADMIN\", \r\n    \"well\": {\r\n        \"url\": \"https://pocoweb.petro.intelie.net/api/public/well/144/\", \r\n        \"name\": \"caso Mateus/Roger\"\r\n    }, \r\n    \"revision\": {\r\n        \"created_at\": \"2018-08-14T14:56:42.105Z\", \r\n        \"schema\": \"dadosbasicos.drilling - 2014-09-09\", \r\n        \"author\": {\r\n            \"url\": \"https://pocoweb.petro.intelie.net/api/public/user/1/\", \r\n            \"name\": \"\"\r\n        }, \r\n        \"description\": \"\", \r\n        \"source\": {\r\n            \"url\": \"https://pocoweb.petro.intelie.net/api/public/application/7/\", \r\n            \"name\": \"DADOSBASICOS\"\r\n        }, \r\n        \"content\": {\r\n       \"input\": {\r\n\t\t\t\t\"airgap\": -25,\r\n                \"lda\": 2110, \r\n                \"abandonment\": {\r\n                    \"reservoir_temperature\": 68, \r\n                    \"remainings\": [], \r\n                    \"cement_plugs\": [\r\n                        {\r\n       \"bpp\": false, \r\n                            \"base_md\": 4769, \r\n                            \"top_md\": 4649, \r\n                            \"plug_type\": \"cement\", \r\n                            \"through_tubing\": false\r\n                        }\r\n                    ], \r\n                    \"wear_plug\": false, \r\n                    \"well_bottom_md\": 5285,\r\n\t\t\t\t}\r\n\t\t}\r\n\t\t}\r\n\t\t}\r\n\t\t}";

            poço.DadosGerais.Geometria.OffShore.LaminaDagua = 10;
            poço.DadosGerais.Geometria.MesaRotativa = 10;

            var dadosSelecionado = A.Fake<List<DadosSelecionadosEnum>>();
            var dadosExtras = A.Fake<Dictionary<string, object>>();
            var listaLitologia = A.Fake<List<LitologiaModel>>();
            var listaPerfis = A.Fake<List<PerfilModel>>();

            dadosExtras.Add("poçoWeb", true);
            dadosExtras.Add("token", "tokenFake");
            dadosSelecionado.Add(DadosSelecionadosEnum.DadosGerais);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            var registroReadOnlyRepository = A.Fake<IRegistrosEventosReadOnlyRepository>();
            var registroWriteOnlyRepository = A.Fake<IRegistrosEventosWriteOnlyRepository>();
            var poçoWebService = A.Fake<IPocoWebService>();

            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult(poço));
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarLitologias(A<string>.Ignored, A<Poço>.Ignored)).Returns(true);
            A.CallTo(() => poçoWebService.GetPoçoWebDTOFromWebByFile(A<string>.Ignored, A<string>.Ignored)).Returns(json);

            var useCase = new ImportarArquivoUseCase(poçoWriteOnlyRepository, poçoReadOnlyRepository, poçoWebService, registroReadOnlyRepository, registroWriteOnlyRepository);

            // Act
            var result = await useCase.Execute(dadosSelecionado, "caminho", listaPerfis, listaLitologia, id, "", dadosExtras);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ImportarArquivoStatus.ImportaçãoComFalhasDeValidação);
        }

        [Test]
        public async Task DeveRetornarSucesso()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var poço = PoçoFactory.CriarPoço(id, "poçoTeste", TipoPoço.Projeto);
            string json = "{\r\n    \"url\": \"https://pocoweb.petro.intelie.net/api/public/file/1819/?rev=7573\", \r\n    \"name\": \"caso mateus_roger\", \r\n    \"file_type\": \"dadosbasicos.drilling\", \r\n    \"description\": \"teste score\", \r\n    \"archived\": false, \r\n    \"created_at\": \"2018-08-14T14:56:42.100Z\", \r\n    \"last_updated_at\": \"2018-08-14T14:56:42.110Z\", \r\n    \"last_updated_by\": \"ADMIN\", \r\n    \"well\": {\r\n        \"url\": \"https://pocoweb.petro.intelie.net/api/public/well/144/\", \r\n        \"name\": \"caso Mateus/Roger\"\r\n    }, \r\n    \"revision\": {\r\n        \"created_at\": \"2018-08-14T14:56:42.105Z\", \r\n        \"schema\": \"dadosbasicos.drilling - 2014-09-09\", \r\n        \"author\": {\r\n            \"url\": \"https://pocoweb.petro.intelie.net/api/public/user/1/\", \r\n            \"name\": \"\"\r\n        }, \r\n        \"description\": \"\", \r\n        \"source\": {\r\n            \"url\": \"https://pocoweb.petro.intelie.net/api/public/application/7/\", \r\n            \"name\": \"DADOSBASICOS\"\r\n        }, \r\n        \"content\": {\r\n       \"input\": {\r\n\t\t\t\t\"airgap\": 25,\r\n                \"lda\": 2110, \r\n                \"abandonment\": {\r\n                    \"reservoir_temperature\": 68, \r\n                    \"remainings\": [], \r\n                    \"cement_plugs\": [\r\n                        {\r\n                            \"bpp\": false, \r\n                            \"base_md\": 4769, \r\n                            \"top_md\": 4649, \r\n                            \"plug_type\": \"cement\", \r\n                            \"through_tubing\": false\r\n                        }\r\n                    ], \r\n                    \"wear_plug\": false, \r\n                    \"well_bottom_md\": 5285,\r\n\t\t\t\t}\r\n\t\t}\r\n\t\t}\r\n\t\t}\r\n\t\t}";

            poço.DadosGerais.Geometria.OffShore.LaminaDagua = 10;
            poço.DadosGerais.Geometria.MesaRotativa = 10;

            var dadosSelecionado = A.Fake<List<DadosSelecionadosEnum>>();
            var dadosExtras = A.Fake<Dictionary<string, object>>();
            var listaLitologia = A.Fake<List<LitologiaModel>>();
            var listaPerfis = A.Fake<List<PerfilModel>>();

            dadosExtras.Add("poçoWeb", true);
            dadosExtras.Add("token", "tokenFake");
            dadosSelecionado.Add(DadosSelecionadosEnum.DadosGerais);
            
            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var poçoWriteOnlyRepository = A.Fake<IPoçoWriteOnlyRepository>();
            var registroReadOnlyRepository = A.Fake<IRegistrosEventosReadOnlyRepository>();
            var registroWriteOnlyRepository = A.Fake<IRegistrosEventosWriteOnlyRepository>();
            var poçoWebService = A.Fake<IPocoWebService>();

            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult(poço));
            A.CallTo(() => poçoWriteOnlyRepository.AtualizarLitologias(A<string>.Ignored, A<Poço>.Ignored)).Returns(true);
            A.CallTo(() => poçoWebService.GetPoçoWebDTOFromWebByFile(A<string>.Ignored, A<string>.Ignored)).Returns(json);

            var useCase = new ImportarArquivoUseCase(poçoWriteOnlyRepository, poçoReadOnlyRepository, poçoWebService, registroReadOnlyRepository, registroWriteOnlyRepository);

            // Act
            var result = await useCase.Execute(dadosSelecionado,"caminho", listaPerfis, listaLitologia, id, "", dadosExtras);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ImportarArquivoStatus.ImportadoComSucesso);
        }
    }
}
