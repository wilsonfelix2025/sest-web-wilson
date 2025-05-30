using System.Collections.Generic;
using System.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.DTOs.InserirTrecho;
using SestWeb.Domain.Entities.DadosGeraisDoPoco;
using SestWeb.Domain.Entities.Perfis.Factory.Generic;
using SestWeb.Domain.Entities.Perfis.TiposPerfil;
using SestWeb.Domain.Entities.PontosEntity;
using SestWeb.Domain.Entities.ProfundidadeEntity;
using SestWeb.Domain.Enums;
using SestWeb.Domain.Factories;
using SestWeb.Domain.Validadores.DTO;

namespace SestWeb.Domain.Tests.ValidadoresTests
{
    [TestFixture]
    public class InserirTrechoTests
    {
        [Test]
        public void DeveRetornarErroQuandoPmLimiteMaiorQueÚltimoPontoPerfil()
        {
            const string nomePerfil1 = "DTC1";
            var poço = PoçoFactory.CriarPoço("1", "1", TipoPoço.Projeto);
            var perfil = PerfisFactory<DTC>.Create(nomePerfil1, poço.Trajetória, poço.ObterLitologiaPadrão()); 
            var listaPontoPM = new List<double> {2000, 3000};
            var listaPontoValor = new List<double> {10,20};

            perfil.AddPontosEmPm(poço.Trajetória, listaPontoPM,listaPontoValor,TipoProfundidade.PM,OrigemPonto.Completado);
            
            var dto = new InserirTrechoDTO
            {
                TipoDeTrecho = TipoDeTrechoEnum.Inicial,
                PerfilSelecionado = perfil,
                PmLimite = 5000,
                TipoTratamentoTrecho = TipoTratamentoTrechoEnum.Linear,
                BaseDeSedimentos = 100,
                NovoNome = "perfilComTrecho",
                ValorTopo = 180,
                ÚltimoPontoTrajetória = 5000,
            };

            var validator = new InserirTrechoValidator();
            var result = validator.Validate(dto);

            Check.That(result).IsNotNull();
            Check.That(result.IsValid).Equals(false);
            Check.That(result.Errors.First().ToString()).IsEqualTo("Profundidade limite maior que último ponto do perfil");
        }
    }
}
