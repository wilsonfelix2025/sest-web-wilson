using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.LitologiaDoPoco;
using SestWeb.Domain.Entities.Trajetoria;
using SestWeb.Domain.Enums;
using SestWeb.Domain.InserirTrecho;
using FakeItEasy;
using SestWeb.Domain.DTOs.InserirTrecho;
using SestWeb.Domain.Entities.Perfis.Factory;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;

namespace SestWeb.Domain.Tests.InserirTrecho
{
    [TestFixture]
    public class InserirTrechoTests
    {
        [Test]
        public void DeveInserirTrechoComSucesso()
        {
            const string nomePerfil1 = "DTC1";
            
            //var perfilFactory = PerfilOld.GetFactory(A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());
            var perfil = PerfisFactory.Create("DTC", nomePerfil1, A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());

            perfil.AddPontoEmPm(perfil.ConversorProfundidade, new Profundidade(2000), 10, TipoProfundidade.PM, OrigemPonto.Importado);
            perfil.AddPontoEmPm(perfil.ConversorProfundidade, new Profundidade(3000), 20, TipoProfundidade.PM, OrigemPonto.Importado);

            var dto = new InserirTrechoDTO
            {
                TipoDeTrecho = TipoDeTrechoEnum.Inicial,
                PerfilSelecionado = perfil,
                PmLimite = 1000,
                TipoTratamentoTrecho = TipoTratamentoTrechoEnum.Linear,
                BaseDeSedimentos = 100,
                NovoNome = "perfilComTrecho",
                ValorTopo = 180,
                ÚltimoPontoTrajetória = 5000
            };

            var inserirTrecho = new InserirTrechos(perfil.ConversorProfundidade, perfil.Litologia, dto);

            var result = inserirTrecho.InserirComplementoDeCurva();

            Check.That(result).IsNotNull();
            Check.That(result.Entity).IsNotNull();
            Check.That(result.result.IsValid).Equals(true);
        }

        [Test]
        public void DeveInserirTrechoComFalha()
        {
            const string nomePerfil1 = "DTC1";

            //var perfilFactory = PerfilOld.GetFactory(A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());
            var perfil = PerfisFactory.Create("DTC", nomePerfil1, A.Fake<IConversorProfundidade>(), A.Fake<ILitologia>());

            perfil.AddPontoEmPm(perfil.ConversorProfundidade, new Profundidade(-10), 0, TipoProfundidade.PM, OrigemPonto.Importado);
            perfil.AddPontoEmPm(perfil.ConversorProfundidade, new Profundidade(3000), 0, TipoProfundidade.PM, OrigemPonto.Importado);

            var dto = new InserirTrechoDTO
            {
                TipoDeTrecho = TipoDeTrechoEnum.Inicial,
                //PerfilOldSelecionado = perfil,
                PmLimite = 1000,
                TipoTratamentoTrecho = TipoTratamentoTrechoEnum.Linear,
                BaseDeSedimentos = 100,
                NovoNome = "perfilComTrecho",
                ValorTopo = 180,
                ÚltimoPontoTrajetória = 5000
            };

            var inserirTrecho = new InserirTrechos(perfil.ConversorProfundidade, null, dto);

            var result = inserirTrecho.InserirComplementoDeCurva();

            Check.That(result).IsNotNull();
            Check.That(result.Entity).IsNull();
            Check.That(result.result.IsValid).Equals(false);
        }
    }
}
