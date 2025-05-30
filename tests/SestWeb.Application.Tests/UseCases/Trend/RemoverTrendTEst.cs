using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.TrendUseCases.RemoverTrend;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.Trend
{
    [TestFixture]
    public class RemoverTrendTest
    {

        [Test]
        public async Task DeveRetornarTrendRemovidoSeNãoHouveProblema()
        {
            // Arrange
            const string idPerfil = "id";
            const string nome = "NovoPerfil";
            const string mnemônico = "DTC";
            var poço = PoçoFactory.CriarPoço("idPoço", "nomePoço", TipoPoço.Projeto);
            var trajetória = poço.Trajetória;
            var litologias = poço.Litologias;
            var litologia = litologias?.Single(x => x.Classificação == TipoLitologia.Adaptada);
            var perfil = PerfisFactory.Create(mnemônico, nome, trajetória, litologia);

            var listaPontoPM = new List<double> { 10, 10 };
            var listaPontoValor = new List<double> { 20, 20 };
            perfil.AddPontosEmPm(poço.Trajetória, listaPontoPM, listaPontoValor, TipoProfundidade.PM, OrigemPonto.Importado);

            var factoryTrend = new TrendFactory();
            var resultTrend = factoryTrend.CriarTrend(perfil, poço);
            perfil.Trend = (Domain.Entities.Trend.Trend)resultTrend.Entity;
            
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();

            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(Task.FromResult(perfil));

            var useCase = new RemoverTrendUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository);

            // Act
            var result = await useCase.Execute(idPerfil);

            //// Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(RemoverTrendStatus.TrendRemovido);
            Check.That(result.Mensagem).IsEqualTo("Trend removido com sucesso");
        }

        [Test]
        public async Task DeveRetornarTrendNãoRemovidoSeHouveProblema()
        {
            // Arrange
            const string mensagemDeErro = "Mensagem de erro.";
            const string idPerfil = "id";
            const string nome = "NovoPerfil";
            const string mnemônico = "DTC";
            var poço = PoçoFactory.CriarPoço("idPoço", "nomePoço", TipoPoço.Projeto);
            var trajetória = poço.Trajetória;
            var litologias = poço.Litologias;
            var litologia = litologias?.Single(x => x.Classificação == TipoLitologia.Adaptada);
            var perfil = PerfisFactory.Create(mnemônico, nome, trajetória, litologia);

            var listaPontoPM = new List<double> { 10, 10 };
            var listaPontoValor = new List<double> { 20, 20 };
            perfil.AddPontosEmPm(poço.Trajetória, listaPontoPM, listaPontoValor, TipoProfundidade.PM, OrigemPonto.Importado);

            var factoryTrend = new TrendFactory();
            var resultTrend = factoryTrend.CriarTrend(perfil, poço);
            perfil.Trend = (Domain.Entities.Trend.Trend)resultTrend.Entity;
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();

            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(Task.FromResult<PerfilBase>(perfil));
            A.CallTo(() => perfilWriteOnlyRepository.AtualizarTrendDoPerfil(A<PerfilBase>.Ignored)).ThrowsAsync(new Exception(mensagemDeErro));
            var useCase = new RemoverTrendUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository);

            // Act
            var result = await useCase.Execute(idPerfil);

            //// Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(RemoverTrendStatus.TrendNãoRemovido);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível remover trend. {mensagemDeErro}");
        }

        [Test]
        public async Task DeveRetornarTrendNãoRemovidoSePerfilNãoExiste()
        {
            // Arrange
            const string idPerfil = "id";

            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();

            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(Task.FromResult<PerfilBase>(null));

            var useCase = new RemoverTrendUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository);

            // Act
            var result = await useCase.Execute(idPerfil);

            //// Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(RemoverTrendStatus.TrendNãoRemovido);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível remover trend. Perfil não encontrado");
        }

    }
}
