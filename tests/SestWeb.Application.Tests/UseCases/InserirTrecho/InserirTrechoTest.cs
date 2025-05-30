
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.InserirTrechoUseCase;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Factories;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.Poço;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Enums;

namespace SestWeb.Application.Tests.UseCases.InserirTrecho
{
    [TestFixture]
    public class InserirTrechoTest
    {
        [Test]
        public async Task DeveRetornarTrechoInseridoComSucesso()
        {
            // Arrange
            const string id = "id";
            var mensagem = "Trecho inserido com sucesso.";
            var input = new InserirTrechoInput
            {
                PMLimite = 5,
                NomeNovoPerfil = "testeperfil",
                TipoDeTrecho = TipoDeTrechoEnum.Inicial,
                TipoTratamento = TipoTratamentoTrechoEnum.Linear
            };
            const string nomePerfil1 = "DTC1";

            var poço = PoçoFactory.CriarPoço("id1", "teste", TipoPoço.Projeto);
            var perfil1 = PerfisFactory.Create("DTC", nomePerfil1, poço.Trajetória, A.Fake<ILitologia>());
            
            var listaPontoPM = new List<double> {10,20};
            var listaPontoValor = new List<double>{10,20};
            perfil1.AddPontosEmPm(poço.Trajetória, listaPontoPM, listaPontoValor,TipoProfundidade.PM, OrigemPonto.Importado);

            //TODO(RCM): Ajustar teste devido alteração feita na mudança dos modelos.

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();

            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(poço);
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(perfil1);
            A.CallTo(() => perfilWriteOnlyRepository.CriarPerfil(A<string>.Ignored, A<PerfilBase>.Ignored, A<Poço>.Ignored)).Returns(Task.CompletedTask);

            var useCase = new InserirTrechoUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository, poçoReadOnlyRepository);

            // Act
            var result = await useCase.Execute(id,input);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(InserirTrechoStatus.InserirTrechoComSucesso);
            Check.That(result.Mensagem).IsEqualTo(mensagem);
        }

        [Test]
        public async Task DeveRetornarTrechoInseridoComFalhaDeValidaçãoDadosDeEntradaVazio()
        {
            // Arrange
            const string id = "id";
            var mensagem = "Trecho inicial inserido com sucesso.";
            var input = new InserirTrechoInput();
            input = null;
            const string nomePerfil1 = "DTC1";

           var perfil = PerfisFactory.Create("DTC", nomePerfil1, A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());
            var poço = PoçoFactory.CriarPoço("id1", "teste", TipoPoço.Projeto);

            var listaPontoPM = new List<double> { 10, 10 };
            var listaPontoValor = new List<double> { 20, 20 };
            perfil.AddPontosEmPm(poço.Trajetória, listaPontoPM,listaPontoValor,TipoProfundidade.PM,OrigemPonto.Importado);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();

            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(poço);
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(perfil);
            A.CallTo(() => perfilWriteOnlyRepository.CriarPerfil(A<string>.Ignored, A<PerfilBase>.Ignored, A<Poço>.Ignored)).Returns(Task.CompletedTask);

            var useCase = new InserirTrechoUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository, poçoReadOnlyRepository);

            // Act
            var result = await useCase.Execute(id, input);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(InserirTrechoStatus.InserirTrechoComFalhaDeValidação);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível inserir trecho. Dados de entrada não preenchidos");
        }

        [Test]
        public async Task DeveRetornarTrechoInseridoComFalhaDeValidaçãoDadosPMLimite()
        {
            // Arrange
            const string id = "id";
            var input = new InserirTrechoInput
            {
                PMLimite = 1000,
                NomeNovoPerfil = "testeperfil",
                TipoDeTrecho = TipoDeTrechoEnum.Inicial,
                TipoTratamento = TipoTratamentoTrechoEnum.Linear
            };

            const string nomePerfil1 = "DTC1";

            var poço = PoçoFactory.CriarPoço("id1", "teste", TipoPoço.Projeto);
            var perfil1 = PerfisFactory.Create("DTC", nomePerfil1, poço.Trajetória, A.Fake<ILitologia>());
            

            var pontosTrajetória = new List<PontoDeTrajetória>();
            pontosTrajetória.Add(new PontoDeTrajetória(1000,0,0));
            pontosTrajetória.Add(new PontoDeTrajetória(2000, 0, 0));
            pontosTrajetória.Add(new PontoDeTrajetória(3000, 0, 0));
            poço.Trajetória.Reset(pontosTrajetória);

            var listaPontoPM = new List<double> { 10, 20 };
            var listaPontoValor = new List<double> { 10, 20 };
            perfil1.AddPontosEmPm(poço.Trajetória, listaPontoPM,listaPontoValor,TipoProfundidade.PM,OrigemPonto.Importado);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();

            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(poço);
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(perfil1);
            A.CallTo(() => perfilWriteOnlyRepository.CriarPerfil(A<string>.Ignored, A<PerfilBase>.Ignored, A<Poço>.Ignored)).Returns(Task.CompletedTask);

            var useCase = new InserirTrechoUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository, poçoReadOnlyRepository);

            // Act
            var result = await useCase.Execute(id, input);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(InserirTrechoStatus.InserirTrechoComFalhaDeValidação);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível inserir trecho. Profundidade limite maior que último ponto do perfil");
        }

        [Test]
        public async Task DeveRetornarTrechoInseridoComFalhaDeValidaçãoDadosNomeRepetido()
        {
            // Arrange
            const string id = "id";
            var input = new InserirTrechoInput
            {
                PMLimite = 5,
                NomeNovoPerfil = "DTC1",
                TipoDeTrecho = TipoDeTrechoEnum.Inicial,
                TipoTratamento = TipoTratamentoTrechoEnum.Linear
            };
            const string nomePerfil1 = "DTC1";
            var perfil1 = PerfisFactory.Create("DTC", nomePerfil1, A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());
            var poço = PoçoFactory.CriarPoço("id1", "teste", TipoPoço.Projeto);

            var pontosTrajetória = new List<PontoDeTrajetória>();
            pontosTrajetória.Add(new PontoDeTrajetória(1000, 0, 0));
            pontosTrajetória.Add(new PontoDeTrajetória(2000, 0, 0));
            pontosTrajetória.Add(new PontoDeTrajetória(3000, 0, 0));
            poço.Trajetória.Reset(pontosTrajetória);

            var listaPontoPM = new List<double> { 10, 10 };
            var listaPontoValor = new List<double> { 20, 20 };
            perfil1.AddPontosEmPm(poço.Trajetória, listaPontoPM,listaPontoValor,TipoProfundidade.PM,OrigemPonto.Importado);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();

            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(poço);
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(perfil1);
            A.CallTo(() => perfilWriteOnlyRepository.CriarPerfil(A<string>.Ignored, A<PerfilBase>.Ignored, A<Poço>.Ignored)).Returns(Task.CompletedTask);
            A.CallTo(() => perfilReadOnlyRepository.ExistePerfilComMesmoNome(A<string>.Ignored, A<string>.Ignored)).Returns(true);

            var useCase = new InserirTrechoUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository, poçoReadOnlyRepository);

            // Act
            var result = await useCase.Execute(id, input);

            // Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(InserirTrechoStatus.InserirTrechoComFalhaDeValidação);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível inserir trecho. " + nomePerfil1 + " - já existe perfil com esse nome");
        }
    }
}
