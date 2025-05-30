using System;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfil;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Application.Tests.UseCases.ObterPerfil
{
    [TestFixture]
    public class ObterPerfilTest
    {
        [Test]
        public async Task DeveRetornarPerfilSeEncontrouPerfil()
        {
            // Arrange
            const string id = "id";
            const string nomePerfil = "DTC";

            //var perfilFactory = PerfilOld.GetFactory(A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());
            var perfil = PerfisFactory.Create("DTC", nomePerfil, A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());

            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(perfil);

            var obterPerfilUseCase = new ObterPerfilUseCase(perfilReadOnlyRepository);

            // Act
            var result = await obterPerfilUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterPerfilStatus.PerfilObtido);
            Check.That(result.Mensagem).IsEqualTo("Perfil obtido com sucesso.");
            Check.That(result.PerfilOld).IsNotNull();
        }

        [Test]
        public async Task DeveRetornarPerfilNãoEncontradoSeNãoEncontrouPerfil()
        {
            // Arrange
            const string id = "id";

            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(Task.FromResult<PerfilBase>(null));

            var obterPerfilUseCase = new ObterPerfilUseCase(perfilReadOnlyRepository);

            // Act
            var result = await obterPerfilUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterPerfilStatus.PerfilNãoEncontrado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível encontrar perfil com id {id}.");
            Check.That(result.PerfilOld).IsNull();
        }

        [Test]
        public async Task DeveRetornarPerfilNãoObtidoSeHouveExceção()
        {
            // Arrange
            const string id = "id";
            const string mensagem = "Mensagem da exceção";

            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).ThrowsAsync(new Exception(mensagem));

            var obterPerfilUseCase = new ObterPerfilUseCase(perfilReadOnlyRepository);

            // Act
            var result = await obterPerfilUseCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterPerfilStatus.PerfilNãoObtido);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível obter perfil. {mensagem}");
            Check.That(result.PerfilOld).IsNull();
        }
    }
}