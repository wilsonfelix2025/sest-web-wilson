using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SestWeb.Api.UseCases.Usuário.RegistrarUsuario;
using SestWeb.Application.UseCases.UsuárioUseCases.RegistrarUsuario;

namespace SestWeb.Api.Tests.UseCases.RegistrarUsuario
{
    [TestFixture]
    public class RegistrarUsuarioTest
    {
        [Test]
        public void ControllerDeveTerAtributoApiController()
        {
            Check.That(Attribute.IsDefined(typeof(UsuarioController), typeof(ApiControllerAttribute))).IsTrue();
        }

        [Test]
        public void ControllerDeveTerAtributoRoute()
        {
            var attribute =
                (RouteAttribute)Attribute.GetCustomAttribute(typeof(UsuarioController), typeof(RouteAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(attribute.Template).IsEqualTo("api/usuarios");
        }

        [Test]
        public void RegistrarUsuarioDeveTerAtributoHttpPost()
        {
            var attribute = (HttpPostAttribute)Attribute.GetCustomAttribute(typeof(UsuarioController).GetMethod(nameof(UsuarioController.RegistrarUsuario)), typeof(HttpPostAttribute));

            Check.That(attribute).IsNotNull();
        }

        [TestCase(201)]
        [TestCase(400)]
        public void RegistrarUsuarioDeveTerAtributoProducesResponseType(int statusCode)
        {
            var attribute = Attribute.GetCustomAttributes(typeof(UsuarioController).GetMethod(nameof(UsuarioController.RegistrarUsuario)), typeof(ProducesResponseTypeAttribute));

            Check.That(attribute).IsNotNull();
            Check.That(((ProducesResponseTypeAttribute[])attribute).ToList()).HasElementThatMatches(at => at.StatusCode == statusCode);
        }

        [Test]
        public async Task RegistrarUsuarioDeveRetornarBadRequestObjectResultEmCasoDeFalha()
        {
            // Arrange
            const string nome = "Cleiton";
            const string sobrenome = "Rodrigues";
            const string email = "cleiton@puc-rio.br";
            const string senha = "Gtep@1234";

            var registrarUsuarioRequest = new RegistrarUsuarioRequest { Nome = nome, Sobrenome = sobrenome, Email = email, Senha = senha };

            var registrarUsuarioUseCase = A.Fake<IRegistrarUsuarioUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new UsuarioController(registrarUsuarioUseCase, presenter);

            A.CallTo(() => registrarUsuarioUseCase.Execute(nome, sobrenome, email, senha)).Returns(RegistrarUsuarioOutput.UsuarioNãoRegistrado("Não deve registrar usuário sem todos os campos preenchidos."));

            // Act
            var result = await controller.RegistrarUsuario(registrarUsuarioRequest);

            // Assert
            Check.That(result).IsInstanceOf<BadRequestObjectResult>();
        }

        [Test]
        public async Task RegistrarUsuarioDeveRetornarCreatedResultEmCasoDeSucesso()
        {
            // Arrange
            const string nome = "Cleiton";
            const string sobrenome = "Rodrigues";
            const string email = "cleiton@puc-rio.br";
            const string senha = "Gtep@1234";

            var registrarUsuarioRequest = new RegistrarUsuarioRequest { Nome = nome, Sobrenome = sobrenome, Email = email, Senha = senha };

            var registrarUsuarioUseCase = A.Fake<IRegistrarUsuarioUseCase>();
            var presenter = A.Fake<Presenter>();
            var controller = new UsuarioController(registrarUsuarioUseCase, presenter);

            A.CallTo(() => registrarUsuarioUseCase.Execute(nome, sobrenome, email, senha)).Returns(RegistrarUsuarioOutput.UsuarioRegistradoComSucesso(new Usuario("id", nome, sobrenome, email, false, false, new DateTime(), new List<string>())));

            // Act
            var result = await controller.RegistrarUsuario(registrarUsuarioRequest);

            // Assert
            Check.That(result).IsInstanceOf<CreatedResult>();
        }
    }
}