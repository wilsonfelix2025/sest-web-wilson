
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Factories;
using SestWeb.Domain.Validadores;
using System.Linq;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Entities.Trend;

namespace SestWeb.Domain.Tests.ValidadoresTests
{
    [TestFixture]
    public class TrendValidatorTest
    {
        [Test]
        public void DeveRetornarErroQuandoPerfilNãoPermiteTrend()
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
            //var factory = PerfilOld.GetFactory(trajetória, litologia);
            //var tipoPerfil = PerfilOld.ObterTipoPerfilPorMnemônico(mnemônico);
            //var perfil = factory.CriarPerfil(tipoPerfil, nome);
            var perfil = PerfisFactory.Create(mnemônico, nome, trajetória, litologia);

            perfil.AddPontoEmPm(trajetória, new Profundidade(10),10,TipoProfundidade.PM,OrigemPonto.Importado );
            perfil.AddPontoEmPm(trajetória, new Profundidade(20), 20, TipoProfundidade.PM, OrigemPonto.Importado);

            //var pontoPerfil = new PontoOld(10, 10, OrigemPonto.Importado);
            //var pontoPerfil2 = new PontoOld(20, 20, OrigemPonto.Importado);
            //var listaPontoPerfil = new List<PontoOld>();
            //listaPontoPerfil.Add(pontoPerfil);
            //listaPontoPerfil.Add(pontoPerfil2);
            //perfil.PontosDePerfil.Reset(listaPontoPerfil);


            var validator = new TrendValidator(poço, false);
            var result = validator.Validate(perfil);

            Check.That(result).IsNotNull();
            Check.That(result.IsValid).Equals(false);
            Check.That(result.Errors.First().ToString()).IsEqualTo("Perfil não permite criação de trend");
        }

        [Test]
        public void DeveRetornarErroQuandoPerfilNãoTemNoMínimoDoisPontos()
        {
            // Arrange
            const string idPerfil = "id";
            const string nome = "NovoPerfil";
            const string mnemônico = "DTC";
            const string mensagem = "Perfil não permite criação de trend";
            var poço = PoçoFactory.CriarPoço("idPoço", "nomePoço", TipoPoço.Projeto);
            var trajetória = poço.Trajetória;
            var litologias = poço.Litologias;
            var litologia = litologias?.Single(x => x.Classificação == TipoLitologia.Adaptada);
            //var factory = PerfilOld.GetFactory(trajetória, litologia);
            //var tipoPerfil = PerfilOld.ObterTipoPerfilPorMnemônico(mnemônico);
            //var perfil = factory.CriarPerfil(tipoPerfil, nome);
            var perfil = PerfisFactory.Create(mnemônico, nome, trajetória, litologia);
            perfil.AddPontoEmPm(trajetória, new Profundidade(10), 10, TipoProfundidade.PM, OrigemPonto.Importado);

            //var pontoPerfil = new PontoOld(10, 10, OrigemPonto.Importado);
            //var listaPontoPerfil = new List<PontoOld>();
            //listaPontoPerfil.Add(pontoPerfil);
            //perfil.PontosDePerfil.Reset(listaPontoPerfil);


            var validator = new TrendValidator(poço, false);
            var result = validator.Validate(perfil);

            Check.That(result).IsNotNull();
            Check.That(result.IsValid).Equals(false);
            Check.That(result.Errors.First().ToString()).IsEqualTo("Perfil não possui pontos suficientes");
        }

        [Test]
        public void DeveRetornarErroQuandoTentaCriarTrendParaPerfilQueJáPossui()
        {
            // Arrange
            const string idPerfil = "id";
            const string nome = "NovoPerfil";
            const string mnemônico = "DTC";
            const string mensagem = "Perfil não permite criação de trend";
            var poço = PoçoFactory.CriarPoço("idPoço", "nomePoço", TipoPoço.Projeto);
            var trajetória = poço.Trajetória;
            var litologias = poço.Litologias;
            var litologia = litologias?.Single(x => x.Classificação == TipoLitologia.Adaptada);
            //var factory = PerfilOld.GetFactory(trajetória, litologia);
            //var tipoPerfil = PerfilOld.ObterTipoPerfilPorMnemônico(mnemônico);
            //var perfil = factory.CriarPerfil(tipoPerfil, nome);
            var perfil = PerfisFactory.Create(mnemônico, nome, trajetória, litologia);
            perfil.AddPontoEmPm(trajetória, new Profundidade(10), 10, TipoProfundidade.PM, OrigemPonto.Importado);
            perfil.AddPontoEmPm(trajetória, new Profundidade(20), 20, TipoProfundidade.PM, OrigemPonto.Importado);

            //var pontoPerfil = new PontoOld(10, 10, OrigemPonto.Importado);
            //var pontoPerfil2 = new PontoOld(20, 20, OrigemPonto.Importado);
            //var listaPontoPerfil = new List<PontoOld>();
            //listaPontoPerfil.Add(pontoPerfil);
            //listaPontoPerfil.Add(pontoPerfil2);
            //perfil.PontosDePerfil.Reset(listaPontoPerfil);
            var trendFactory = new TrendFactory();
            var trend = trendFactory.CriarTrend(perfil, poço);
            perfil.Trend = (Trend)trend.Entity;

            var validator = new TrendValidator(poço, false);
            var result = validator.Validate(perfil);

            Check.That(result).IsNotNull();
            Check.That(result.IsValid).Equals(false);
            Check.That(result.Errors.First().ToString()).IsEqualTo("Perfil já possui Trend");
        }

        [Test]
        public void DeveRetornarErroQuandoPrimeiroPontoForDentroDaBaseDeSedimentos()
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
            //var factory = PerfilOld.GetFactory(trajetória, litologia);
            //var tipoPerfil = PerfilOld.ObterTipoPerfilPorMnemônico(mnemônico);
            //var perfil = factory.CriarPerfil(tipoPerfil, nome);
            var perfil = PerfisFactory.Create(mnemônico, nome, trajetória, litologia);
            perfil.AddPontoEmPm(trajetória,new Profundidade(10), 10, TipoProfundidade.PM, OrigemPonto.Importado);
            perfil.AddPontoEmPm(trajetória, new Profundidade(20), 20, TipoProfundidade.PM, OrigemPonto.Importado);

            //var pontoPerfil = new PontoOld(10, 10, OrigemPonto.Importado);
            //var pontoPerfil2 = new PontoOld(20, 20, OrigemPonto.Importado);
            //var listaPontoPerfil = new List<PontoOld>();
            //listaPontoPerfil.Add(pontoPerfil);
            //listaPontoPerfil.Add(pontoPerfil2);
            //perfil.PontosDePerfil.Reset(listaPontoPerfil);
            var factoryTrend = new TrendFactory();
            var resultTrend = factoryTrend.CriarTrend(perfil, poço);
            perfil.Trend = (Trend)resultTrend.Entity;

            var validator = new TrendValidator(poço, true);
            var result = validator.Validate(perfil);

            Check.That(result).IsNotNull();
            Check.That(result.IsValid).Equals(false);
            Check.That(result.Errors.First().ToString()).IsEqualTo("Primeiro ponto menor que base de sedimentos");
        }

    }
}
