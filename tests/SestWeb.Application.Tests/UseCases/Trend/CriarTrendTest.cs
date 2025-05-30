
using System.Collections.Generic;
using FakeItEasy;
using NFluent;
using NUnit.Framework;
using SestWeb.Application.Repositories;
using SestWeb.Application.UseCases.TrendUseCases.CriarTrend;
using System.Threading.Tasks;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Factories;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using System.Linq;
using SestWeb.Domain.Entities.Perfis.Base;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Application.Tests.UseCases.Trend
{
    [TestFixture]
    public class CriarTrendTest
    {

        [Test]
        public async Task DeveRetornarTrendSeTrendFoiCriado()
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

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();

            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult(poço));
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(Task.FromResult(perfil));

            var useCase = new CriarTrendUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository, poçoReadOnlyRepository);

            // Act
            var result = await useCase.Execute(idPerfil);

            //// Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarTrendStatus.TrendCriado);
            Check.That(result.Mensagem).IsEqualTo("Trend criado com sucesso");
            Check.That(result.Trend).IsNotNull();
        }

        [Test]
        public async Task NãoDeveCriarTrendPerfilNãoTemMinimoDeDoisPontos()
        {
            // Arrange
            const string idPerfil = "id";
            const string nome = "NovoPerfil";
            const string mnemônico = "DTC";
            const string mensagem = "Perfil não possui pontos suficientes";
            var poço = PoçoFactory.CriarPoço("idPoço", "nomePoço", TipoPoço.Projeto);
            var trajetória = poço.Trajetória;
            var litologias = poço.Litologias;
            var litologia = litologias?.Single(x => x.Classificação == TipoLitologia.Adaptada);
            var perfil = PerfisFactory.Create(mnemônico, nome, trajetória, litologia);
            var listaPontoPM = new List<double> { 10, 10 };
            var listaPontoValor = new List<double> { 20, 20 };
            perfil.AddPontosEmPm(poço.Trajetória, listaPontoPM, listaPontoValor, TipoProfundidade.PM, OrigemPonto.Importado);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();

            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult(poço));
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(Task.FromResult(perfil));

            var useCase = new CriarTrendUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository, poçoReadOnlyRepository);

            // Act
            var result = await useCase.Execute(idPerfil);

            //// Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarTrendStatus.TrendNãoCriado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível criar trend. {mensagem}");
            Check.That(result.Trend).IsNull();
        }

        [Test]
        public async Task NãoDeveCriarTrendSeNãoEncontrouPerfil()
        {
            // Arrange
            const string idPerfil = "id";
            const string nome = "NovoPerfil";
            const string mnemônico = "DTC";
            const string mensagem = "Perfil não encontrado";
            var poço = PoçoFactory.CriarPoço("idPoço", "nomePoço", TipoPoço.Projeto);
            var trajetória = poço.Trajetória;
            var litologias = poço.Litologias;
            var litologia = litologias?.Single(x => x.Classificação == TipoLitologia.Adaptada);
            var perfil = PerfisFactory.Create(mnemônico, nome, trajetória, litologia);
            var listaPontoPM = new List<double> { 10, 10 };
            var listaPontoValor = new List<double> { 20, 20 };
            perfil.AddPontosEmPm(poço.Trajetória, listaPontoPM, listaPontoValor, TipoProfundidade.PM, OrigemPonto.Importado);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();

            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult(poço));
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(Task.FromResult<PerfilBase>(null));

            var useCase = new CriarTrendUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository, poçoReadOnlyRepository);

            // Act
            var result = await useCase.Execute(idPerfil);

            //// Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarTrendStatus.TrendNãoCriado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível criar trend. {mensagem}");
            Check.That(result.Trend).IsNull();
        }

        [Test]
        public async Task NãoDeveCriarTrendSePerfilNãoPermite()
        {
            // Arrange
            const string idPerfil = "id";
            const string nome = "NovoPerfil";
            const string mnemônico = "GENERICO";
            const string mensagem = "Perfil não permite criação de trend";
            var poço = PoçoFactory.CriarPoço("idPoço", "nomePoço", TipoPoço.Projeto);
            var trajetória = poço.Trajetória;
            var litologias = poço.Litologias;
            var litologia = litologias?.Single(x => x.Classificação == TipoLitologia.Adaptada);
            var perfil = PerfisFactory.Create(mnemônico, nome, trajetória, litologia);

            var listaPontoPM = new List<double> { 10, 10 };
            var listaPontoValor = new List<double> { 20, 20 };
            perfil.AddPontosEmPm(poço.Trajetória, listaPontoPM, listaPontoValor, TipoProfundidade.PM, OrigemPonto.Importado);

            var poçoReadOnlyRepository = A.Fake<IPoçoReadOnlyRepository>();
            var perfilReadOnlyRepository = A.Fake<IPerfilReadOnlyRepository>();
            var perfilWriteOnlyRepository = A.Fake<IPerfilWriteOnlyRepository>();

            A.CallTo(() => poçoReadOnlyRepository.ObterPoço(A<string>.Ignored)).Returns(Task.FromResult(poço));
            A.CallTo(() => perfilReadOnlyRepository.ObterPerfil(A<string>.Ignored)).Returns(Task.FromResult(perfil));

            var useCase = new CriarTrendUseCase(perfilReadOnlyRepository, perfilWriteOnlyRepository, poçoReadOnlyRepository);

            // Act
            var result = await useCase.Execute(idPerfil);

            //// Assert
            Check.That(result).IsNotNull();
            Check.That(result.Status).IsEqualTo(CriarTrendStatus.TrendNãoCriado);
            Check.That(result.Mensagem).IsEqualTo($"Não foi possível criar trend. {mensagem}");
            Check.That(result.Trend).IsNull();
        }

    }
}
