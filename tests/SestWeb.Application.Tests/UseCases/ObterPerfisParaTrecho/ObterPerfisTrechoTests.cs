using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PerfilUseCases.ObterPerfisParaTrecho;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.ObterPerfisParaTrecho
{
    [TestFixture]
    public class ObterPerfisTrechoTests
    {
        [Test]
        public async Task DeveRetornarPerfisObtidosParaTrecho()
        {
            // Arrange
            const string id = "id";
            const string nomePerfil1 = "DTC1";
            const string nomePerfil2 = "DTC2";

            var perfil1 = PerfisFactory.Create("DTC", nomePerfil1, A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());
            var perfil2 = PerfisFactory.Create("DTC", nomePerfil2, A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());
            var poço = PoçoFactory.CriarPoço("id1", "teste", TipoPoço.Projeto);

            var listaPontoPM = new List<double> { 10, 20 };
            var listaPontoValor = new List<double> { 10, 20 };
            perfil1.AddPontosEmPm(poço.Trajetória, listaPontoPM,listaPontoValor,TipoProfundidade.PM,OrigemPonto.Importado);
            perfil2.AddPontosEmPm(poço.Trajetória, listaPontoPM, listaPontoValor, TipoProfundidade.PM, OrigemPonto.Importado);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(poço);
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfisDeUmPoço(A<string>.Ignored)).Returns(new List<PerfilBase> { perfil1, perfil2 });

            var useCase = new ObterPerfisTrechoUseCase(poçoReadOnlyRepository, perfilReadOnlyRepository);

            // Act
            var result = await useCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterPerfisTrechoStatus.PerfisObtidos);
            Check.That(result.Mensagem).IsEqualTo("Perfis obtidos com sucesso.");
            Check.That(result.ObterPerfisTrechoModel.ListaPerfis).CountIs(2);
        }

        [Test]
        public async Task DeveRetornarPerfisNãoObtidosCasoNãoEncontrePoço()
        {
            // Arrange
            const string id = "id";
            var mensagem = $"Não foi possível encontrar poço com id {id}.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult<Poço>(null));

            var useCase = new ObterPerfisTrechoUseCase(poçoReadOnlyRepository, perfilReadOnlyRepository);

            // Act
            var result = await useCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterPerfisTrechoStatus.PerfisNãoObtidos);
            Check.That(result.Mensagem).IsEqualTo(mensagem);
            Check.That(result.ObterPerfisTrechoModel).IsNull();
        }

        [Test]
        public async Task DeveRetornarPerfisNãoObtidosCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagem = "Mensagem de erro.";

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).ThrowsAsync(new Exception(mensagem));
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfisDeUmPoço(A<string>.Ignored)).ThrowsAsync(new Exception(mensagem));

            var useCase = new ObterPerfisTrechoUseCase(poçoReadOnlyRepository, perfilReadOnlyRepository);

            // Act
            var result = await useCase.Execute(id);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterPerfisTrechoStatus.PerfisNãoObtidos);
            Check.That(result.Mensagem).IsEqualTo(mensagem);
            Check.That(result.ObterPerfisTrechoModel).IsNull();
        }
    }
}