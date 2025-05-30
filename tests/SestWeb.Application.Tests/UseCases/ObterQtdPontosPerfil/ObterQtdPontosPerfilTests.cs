
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.PerfilUseCases.ObterQtdPontosPerfil;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Factories;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Enums;
using System;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Application.Tests.UseCases.ObterQtdPontosPerfil
{
    [TestFixture]
    public class ObterQtdPontosPerfilTests
    {

        [Test]
        public async Task DeveRetornarQtdPontosDoPerfil()
        {
            // Arrange
            const string id = "id";
            const string nomePerfil1 = "DTC1";
            var input = new ObterQtdPontosPerfilInput
            {
                LitologiasSelecionadas = null,
                PmLimite = 15,
                TipoTrecho = TipoDeTrechoEnum.Inicial
            };

            var perfil1 = PerfisFactory.Create("DTC", nomePerfil1, A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());
            var poço = PoçoFactory.CriarPoço("id1", "teste", TipoPoço.Projeto);
     
            var listaPontoPM = new List<double> { 10, 10 };
            var listaPontoValor = new List<double> { 20, 20 };
            perfil1.AddPontosEmPm(poço.Trajetória, listaPontoPM, listaPontoValor, TipoProfundidade.PM, OrigemPonto.Importado);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(poço);
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(perfil1);

            var useCase = new ObterQtdPontosPerfilUseCase(perfilReadOnlyRepository, poçoReadOnlyRepository);

            // Act
            var result = await useCase.Execute(id, input);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterQtdPontosPerfilStatus.QtdObtida);
            Check.That(result.Qtd).Equals(1);
        }

        
        [Test]
        public async Task DeveRetornarPerfisNãoObtidosCasoOcorraErro()
        {
            // Arrange
            const string id = "id";
            const string mensagem = "Mensagem de erro.";
            var input = new ObterQtdPontosPerfilInput
            {
                LitologiasSelecionadas = null,
                PmLimite = 15,
                TipoTrecho = TipoDeTrechoEnum.Inicial
            };


            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).ThrowsAsync(new Exception(mensagem));
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).ThrowsAsync(new Exception(mensagem));

            var useCase = new ObterQtdPontosPerfilUseCase(perfilReadOnlyRepository, poçoReadOnlyRepository);

            // Act
            var result = await useCase.Execute(id, input);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterQtdPontosPerfilStatus.QtdNãoObtida);
            Check.That(result.Mensagem).IsEqualTo(mensagem);
        }

    }
}
