
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.TrendUseCases.ObterTrend;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.Trend
{
    [TestFixture]
    public class ObterTrendTest
    {
        [Test]
        public async Task DeveRetornarTrendSeNãoHouveProblema()
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
            var pontosPerfilPM = new List<double> { 10 };
            var pontosPerfilValor = new List<double> { 10 };
            perfil.AddPontosEmPm(poço.Trajetória, pontosPerfilPM, pontosPerfilValor, TipoProfundidade.PM, OrigemPonto.Importado);

            var factoryTrend = new TrendFactory();
            var resultTrend = factoryTrend.CriarTrend(perfil, poço);
            perfil.Trend = (Domain.Entities.Trend.Trend)resultTrend.Entity;

            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();

            A.CallTo(() => perfilReadOnlyRepository.ObterTrendDoPerfil(A<string>.Ignored)).Returns(Task.FromResult(perfil.Trend));

            var useCase = new ObterTrendUseCase(perfilReadOnlyRepository);

            // Act
            var result = await useCase.Execute(idPerfil);

            //// Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterTrendStatus.TrendObtido);
            Check.That(result.Mensagem).IsEqualTo("Trend obtido com sucesso");
            Check.That(result.Trend).IsNotNull();
        }

        [Test]
        public async Task DeveRetornarTrendNãoObtidoSeHouveProblema()
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

            A.CallTo(() => perfilReadOnlyRepository.ObterTrendDoPerfil(A<string>.Ignored)).Returns(Task.FromResult<Domain.Entities.Trend.Trend>(null));

            var useCase = new ObterTrendUseCase(perfilReadOnlyRepository);

            // Act
            var result = await useCase.Execute(idPerfil);

            //// Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(ObterTrendStatus.TrendNãoObtido);
            Check.That(result.Mensagem).IsEqualTo("Não foi possível obter trend. Trend não encontrado");
            Check.That(result.Trend).IsNull();
        }
    }
}
