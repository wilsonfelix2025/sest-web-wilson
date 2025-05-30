
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.CálculosUseCase.Pipeline;
using SestWeb.Application.UseCases.TrendUseCases.EditarTrend;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Factories;

namespace SestWeb.Application.Tests.UseCases.Trend
{
    [TestFixture]
    public class EditarTrendTest
    {
        [Test]
        public async Task DeveRetornarTrendEditadoSeNãoHouveProblema()
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
            var input = new List<EditarTrechosInput>();
            var trechoinput = new EditarTrechosInput
            {
                PvTopo = 10,
                PvBase = 30,
                ValorBase = 200,
                ValorTopo = 100
            };
            input.Add(trechoinput);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();
            var pipe = A.Fake<IPipelineUseCase>();

            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult(poço));
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(Task.FromResult(perfil));

            var useCase = new EditarTrendUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository, poçoReadOnlyRepository, pipe);

            // Act
            var result = await useCase.Execute(input,idPerfil,"nomeTrend");

            //// Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(EditarTrendStatus.TrendEditado);
            Check.That(result.Mensagem).IsEqualTo("Trend editado com sucesso");
            Check.That(result.Trend).IsNotNull();
        }

        [Test]
        public async Task DeveRetornarErroSePrimeiroTrechoForNaBaseDeSedimentos()
        {
            // Arrange
            const string idPerfil = "id";
            const string nome = "NovoPerfil";
            const string mnemônico = "DTC";
            const string mensagem = "Primeiro ponto menor que base de sedimentos";
            var poço = PoçoFactory.CriarPoço("idPoço", "nomePoço", TipoPoço.Projeto);
            poço.DadosGerais.Geometria.OffShore.LaminaDagua = 1000;
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
            var input = new List<EditarTrechosInput>();
            var trechoinput = new EditarTrechosInput
            {
                PvTopo = 10,
                PvBase = 30,
                ValorBase = 200,
                ValorTopo = 100
            };
            input.Add(trechoinput);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();
            var pipe = A.Fake<IPipelineUseCase>();


            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult(poço));
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(Task.FromResult(perfil));

            var useCase = new EditarTrendUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository, poçoReadOnlyRepository, pipe);

            // Act
            var result = await useCase.Execute(input, idPerfil, "nomeTrend");

            //// Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(EditarTrendStatus.TrendNãoEditado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível editar trend. {mensagem}");
            Check.That(result.Trend).IsNull();
        }


        [Test]
        public async Task DeveRetornarErroSeÚltimoTrechoForDepoisDaTrajetória()
        {
            // Arrange
            const string idPerfil = "id";
            const string nome = "NovoPerfil";
            const string mnemônico = "DTC";
            const string mensagem = "Último ponto maior que último ponto da trajetória";
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
            var input = new List<EditarTrechosInput>();
            var trechoinput = new EditarTrechosInput
            {
                PvTopo = 10,
                PvBase = 30000,
                ValorBase = 200,
                ValorTopo = 100
            };
            input.Add(trechoinput);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();
            var pipe = A.Fake<IPipelineUseCase>();


            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult(poço));
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(Task.FromResult(perfil));

            var useCase = new EditarTrendUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository, poçoReadOnlyRepository, pipe);

            // Act
            var result = await useCase.Execute(input, idPerfil, "nomeTrend");

            //// Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(EditarTrendStatus.TrendNãoEditado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível editar trend. {mensagem}");
            Check.That(result.Trend).IsNull();
        }

        [Test]
        public async Task DeveRetornarErroSeTrechosForemInválidos()
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
            var input = new List<EditarTrechosInput>();
            var trechoinput = new EditarTrechosInput
            {
                PvTopo = 10,
                PvBase = 30,
                ValorBase = 200,
                ValorTopo = 100
            };
            var trechoinput2 = new EditarTrechosInput
            {
                PvTopo = 31,
                PvBase = 40,
                ValorBase = 200,
                ValorTopo = 100
            };
            input.Add(trechoinput);
            input.Add(trechoinput2);
            var mensagem = "Trechos inválidos: " + trechoinput2.PvTopo + "/" + trechoinput2.PvBase + " - " + trechoinput.PvTopo + "/" + trechoinput.PvBase;

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();
            var pipe = A.Fake<IPipelineUseCase>();


            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult(poço));
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(Task.FromResult(perfil));

            var useCase = new EditarTrendUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository, poçoReadOnlyRepository, pipe);

            // Act
            var result = await useCase.Execute(input, idPerfil, "nomeTrend");

            //// Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(EditarTrendStatus.TrendNãoEditado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível editar trend. {mensagem}");
            Check.That(result.Trend).IsNull();
        }
    }
}
