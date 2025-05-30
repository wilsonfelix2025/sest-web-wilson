using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PerfilUseCases.CriarPerfil;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.Trajetoria;

namespace SestWeb.Application.Tests.UseCases.CriarPerfil
{
    [TestFixture]
    public class CriarPerfilTest
    {
        [Test]
        public async Task SeTudoOkEntãoDevePassarUmPerfilParaRepositórioUmaVez()
        {
            // Arrange
            const string idPoço = "id";
            const string nome = "NovoPerfil";
            const string mnemônico = "DTC";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();
            IConversorProfundidade trajetória = A.Fake<IConversorProfundidade>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(idPoço)).Returns(true);
            A.CallTo(() => poçoReadOnlyRepository.ObterTrajetória(idPoço)).Returns(new Trajetória(MétodoDeCálculoDaTrajetória.MinimaCurvatura));
            A.CallTo(() => poçoReadOnlyRepository.ObterLitologias(idPoço)).Returns(new List<Litologia> { new Litologia(TipoLitologia.Adaptada, trajetória) });

            var criarPoçoUseCase = new CriarPerfilUseCase(poçoReadOnlyRepository, perfilWriteOnlyRepository);

            // Act
            await criarPoçoUseCase.Execute(idPoço, nome, mnemônico);

            // Assert
            A.CallTo(() => perfilWriteOnlyRepository.CriarPerfil(A<string>.Ignored, A<PerfilBase>.That.Matches(p => p.Nome == nome && p.Mnemonico == mnemônico), A<Poço>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task DeveRetornarPerfilSePerfilFoiCriado()
        {
            // Arrange
            const string idPoço = "id";
            const string nome = "NovoPerfil";
            const string mnemônico = "DTC";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();
            IConversorProfundidade trajetória = A.Fake<IConversorProfundidade>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(idPoço)).Returns(true);
            A.CallTo(() => poçoReadOnlyRepository.ObterTrajetória(idPoço)).Returns(new Trajetória(MétodoDeCálculoDaTrajetória.MinimaCurvatura));
            A.CallTo(() => poçoReadOnlyRepository.ObterLitologias(idPoço)).Returns(new List<Litologia> { new Litologia(TipoLitologia.Adaptada, trajetória) });

            var criarPoçoUseCase = new CriarPerfilUseCase(poçoReadOnlyRepository, perfilWriteOnlyRepository);

            // Act
            var result = await criarPoçoUseCase.Execute(idPoço, nome, mnemônico);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarPerfilStatus.PerfilCriado);
            Check.That(result.Mensagem).IsEqualTo("Perfil criado com sucesso.");
            Check.That(result.PerfilOld).IsNotNull();
            Check.That(result.PerfilOld.Nome).IsEqualTo(nome);
            Check.That(result.PerfilOld.Mnemonico).IsEqualTo(mnemônico);
        }

        [Test]
        public async Task NãoDeveCriarPerfilSeHouveErro()
        {
            // Arrange
            const string idPoço = "id";
            const string nome = "NovoPerfil";
            const string mnemônico = "DTC";
            const string mensagem = "Mensagem de erro.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();
            IConversorProfundidade trajetória = A.Fake<IConversorProfundidade>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(idPoço)).Returns(true);
            A.CallTo(() => poçoReadOnlyRepository.ObterTrajetória(idPoço)).Returns(new Trajetória(MétodoDeCálculoDaTrajetória.MinimaCurvatura));
            A.CallTo(() => poçoReadOnlyRepository.ObterLitologias(idPoço)).Returns(new List<Litologia> { new Litologia(TipoLitologia.Adaptada, trajetória) });
            A.CallTo(() => perfilWriteOnlyRepository.CriarPerfil(A<string>.Ignored, A<PerfilBase>.Ignored, A<Poço>.Ignored)).Throws(new Exception(mensagem));

            var criarPoçoUseCase = new CriarPerfilUseCase(poçoReadOnlyRepository, perfilWriteOnlyRepository);

            // Act
            var result = await criarPoçoUseCase.Execute(idPoço, nome, mnemônico);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarPerfilStatus.PerfilNãoCriado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível criar perfil. {mensagem}");
            Check.That(result.PerfilOld).IsNull();
        }

        [Test]
        public async Task NãoDeveCriarPerfilSeNãoEncontrouPoço()
        {
            // Arrange
            const string idPoço = "id";
            const string nome = "NovoPerfil";
            const string mnemônico = "DTC";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ExistePoço(idPoço)).Returns(false);

            var criarPoçoUseCase = new CriarPerfilUseCase(poçoReadOnlyRepository, perfilWriteOnlyRepository);

            // Act
            var result = await criarPoçoUseCase.Execute(idPoço, nome, mnemônico);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarPerfilStatus.PerfilNãoCriado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível criar perfil. Não foi possível encontrar poço com id {idPoço}.");
            Check.That(result.PerfilOld).IsNull();
        }
    }
}