using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisPorTipo;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Application.Tests.UseCases.ObterPerfisPorTipo
{
    [TestFixture]
    public class ObterPerfisPorTipoTest
    {
        [Test]
        public async Task DeveRetornarPerfisObtidosCasoMnemônicoSejaVálido()
        {
            // Arrange
            const string id = "id";
            const string nomePerfil1 = "DTC1";
            const string nomePerfil2 = "DTC2";

            //var perfilFactory = PerfilOld.GetFactory(A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());
            var perfil1 = PerfisFactory.Create("DTC", nomePerfil1, A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());
            var perfil2 = PerfisFactory.Create("DTC", nomePerfil2, A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());

            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfisPorTipo(A<string>.Ignored, A<string>.Ignored)).Returns(new List<PerfilBase> {perfil1, perfil2});
            

            var obterPerfilUseCase = new ObterPerfisPorTipoUseCase(perfilReadOnlyRepository);

            // Act
            var result = await obterPerfilUseCase.Execute(id, "DTC");

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterPerfisPorTipoStatus.PerfisObtidos);
            Check.That(result.Mensagem).IsEqualTo("Perfis obtidos com sucesso.");
            Check.That(result.Perfis).CountIs(2);
        }

        [Test]
        public async Task DeveRetornarPerfisNãoObtidosCasoMnemônicoSejaInválido()
        {
            // Arrange
            const string id = "id";
            const string mnemônico = "ABC";

            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();

            var obterPerfilUseCase = new ObterPerfisPorTipoUseCase(perfilReadOnlyRepository);

            // Act
            var result = await obterPerfilUseCase.Execute(id, mnemônico);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterPerfisPorTipoStatus.PerfisNãoObtidos);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível obter perfis. Mnemônico inválido - {mnemônico}");
            Check.That(result.Perfis).IsNull();
        }

        [Test]
        public async Task DeveRetornarPerfisNãoObtidosCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mnemônico = "DTC";
            const string mensagem = "Mensagem de erro.";

            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfisPorTipo(A<string>.Ignored, A<string>.Ignored)).ThrowsAsync(new Exception(mensagem));

            var obterPerfilUseCase = new ObterPerfisPorTipoUseCase(perfilReadOnlyRepository);

            // Act
            var result = await obterPerfilUseCase.Execute(id, mnemônico);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterPerfisPorTipoStatus.PerfisNãoObtidos);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível obter perfis. {mensagem}");
            Check.That(result.Perfis).IsNull();
        }
    }
}