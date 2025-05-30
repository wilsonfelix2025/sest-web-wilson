using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisDeUmPoço;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Perfis.Factory.Generic;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Application.Tests.UseCases.ObterPerfisDeUmPoço
{
    [TestFixture]
    public class ObterPerfisTrechoTests
    {
        [Test]
        public async Task DeveRetornarPerfisObtidos()
        {
            // Arrange
            const string id = "id";
            const string nomePerfil1 = "DTC1";
            const string nomePerfil2 = "DTC2";

            //var perfilFactory = PerfilOld.GetFactory(A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());
            var perfil1 = PerfisFactory.Create("DTC", nomePerfil1, A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());
            var perfil2 = PerfisFactory.Create("DTC", nomePerfil2, A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfisDeUmPoço(A<string>.Ignored)).Returns(new List<PerfilBase> { perfil1, perfil2 });

            var obterPerfisPorTipoUseCase = new ObterPerfisDeUmPoçoUseCase(poçoReadOnlyRepository, perfilReadOnlyRepository);

            // Act
            var result = await obterPerfisPorTipoUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterPerfisDeUmPoçoStatus.PerfisObtidos);
            Check.That(result.Mensagem).IsEqualTo("Perfis obtidos com sucesso.");
            Check.That(result.Perfis).CountIs(2);
        }

        [Test]
        public async Task DeveRetornarPerfisNãoObtidosCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";
            var mensagem = $"Não foi possível encontrar poço com id {id}.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(false);

            var obterPerfilUseCase = new ObterPerfisDeUmPoçoUseCase(poçoReadOnlyRepository, perfilReadOnlyRepository);

            // Act
            var result = await obterPerfilUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterPerfisDeUmPoçoStatus.PerfisNãoObtidos);
            Check.That(result.Mensagem).IsEqualTo($"[ObterPerfisDeUmPoço] - {mensagem}");
            Check.That(result.Perfis).IsNull();
        }

        [Test]
        public async Task DeveRetornarPerfisNãoObtidosCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagem = "Mensagem de erro.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(A<string>.Ignored)).Returns(true);
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfisDeUmPoço(A<string>.Ignored)).ThrowsAsync(new Exception(mensagem));

            var obterPerfilUseCase = new ObterPerfisDeUmPoçoUseCase(poçoReadOnlyRepository, perfilReadOnlyRepository);

            // Act
            var result = await obterPerfilUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterPerfisDeUmPoçoStatus.PerfisNãoObtidos);
            Check.That(result.Mensagem).IsEqualTo($"[ObterPerfisDeUmPoço] - {mensagem}");
            Check.That(result.Perfis).IsNull();
        }
    }
}