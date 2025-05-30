using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NFluent;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.TiposVálidosCorrelação;
using SestWeb.Domain.Entities.Correlações.TokensCorrelação;
using SestWeb.Domain.Entities.Correlações.VariáveisCorrelação.Validator;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.Variáveis
{
    [TestFixture]
    public class VariáveisValidatorTests
    {
        private VariáveisValidator _sut;
        private VariáveisTestHelper _variáveis;

        [SetUp]
        public void Setup()
        {
            _sut = new VariáveisValidator();
            _variáveis = new VariáveisTestHelper(string.Empty);
        }

        [TearDown]
        public void TearDown()
        {
            _variáveis?.Clear();
        }

        [Test]
        public void DeveResultarErroQuandoVariáveisÉNull()
        {
            _variáveis = null;
            var result = _sut.Validate(_variáveis);
            Check.That(result.IsValid).IsFalse();
        }

        [Test]
        public void ErrorsCountDeveSer1QuandoVariáveisÉNull()
        {
            _variáveis = null;
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors.Count).IsEqualTo(1);
        }

        [Test]
        public void DeveApresentarMensagemDeVariáveisÉNull()
        {
            _variáveis = null;
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo($"{typeof(Entities.Correlações.VariáveisCorrelação.Variáveis).Name} não pode ser null.");
        }

        [Test]
        public void DeveInvalidarVariávelComNomeNumérico()
        {
            _variáveis.AddVar("1", 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.IsValid).IsFalse();
        }

        [Test]
        public void DeveInvalidarVariávelComNomeNuméricoComMsgCustomizada()
        {
            _variáveis.AddVar("1", 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"1\" inválida - Não é permitido nomes de variáveis com formato numérico!");
        }

        [Test]
        public void DeveInvalidarVariávelComNomeContendoCaracteresEspeciais()
        {
            _variáveis.AddVar("#", 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo($"Variável \"{"#"}\" inválida - Nome contendo caracteres não permitidos!");
        }

        [Test]
        public void DeveReconhecerVariáveisComGrandePrecisão()
        {
            _variáveis.AddVar("a", 0.0000000000000000000000000000001);
            var result = _sut.Validate(_variáveis);
            Check.That(result.IsValid).IsTrue();
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComTipoDePerfil()
        {
            foreach (var perfil in GetTiposPerfil())
            {
                _variáveis.AddVar(perfil, 1);
                var result = _sut.Validate(_variáveis);
                Check.That(result.IsValid).IsFalse();
                _variáveis.Clear();
            }
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComTipoDePerfilComMsgCustomizada()
        {
            _variáveis.AddVar("DTS", 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"DTS\" inválida - Nome reservado para entrada de perfis!");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComTokenDaCorrelação()
        {
            foreach (var token in GetTokens())
            {
                _variáveis.AddVar(token, 1);
                var result = _sut.Validate(_variáveis);
                Check.That(result.IsValid).IsFalse();
                _variáveis.Clear();
            }
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComMesaRotativaComMsgCustomizada()
        {
            _variáveis.AddVar(TokensEnum.MESA_ROTATIVA.ToString(), 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"MESA_ROTATIVA\" inválida - Nome reservado para token de Mesa Rotativa.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComAlturaAntepoçoComMsgCustomizada()
        {
            _variáveis.AddVar(TokensEnum.ALTURA_ANTEPOCO.ToString(), 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"ALTURA_ANTEPOCO\" inválida - Nome reservado para token de Altura do Antepoço.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComCategoriaPoçoComMsgCustomizada()
        {
            _variáveis.AddVar(TokensEnum.CATEGORIA_POCO.ToString(), 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"CATEGORIA_POCO\" inválida - Nome reservado para token de Categoria do Poço.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComDensidadeAguaMarComMsgCustomizada()
        {
            _variáveis.AddVar(TokensEnum.DENSIDADE_AGUA_MAR.ToString(), 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"DENSIDADE_AGUA_MAR\" inválida - Nome reservado para token de Densidade da água do mar.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComGrupoLitologicoComMsgCustomizada()
        {
            _variáveis.AddVar(TokensEnum.GRUPO_LITOLOGICO.ToString(), 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"GRUPO_LITOLOGICO\" inválida - Nome reservado para token de Grupo Litológico.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComLaminaDaguaComMsgCustomizada()
        {
            _variáveis.AddVar(TokensEnum.LAMINA_DAGUA.ToString(), 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"LAMINA_DAGUA\" inválida - Nome reservado para token de Lâmina d'água.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComPoçoOffshoreComMsgCustomizada()
        {
            _variáveis.AddVar(TokensEnum.OFFSHORE.ToString(), 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"OFFSHORE\" inválida - Nome reservado para token de Poço Offshore.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComPoçoOnshoreComMsgCustomizada()
        {
            _variáveis.AddVar(TokensEnum.ONSHORE.ToString(), 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"ONSHORE\" inválida - Nome reservado para token de Poço Onshore.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComProfundidadeComMsgCustomizada()
        {
            _variáveis.AddVar(TokensEnum.PROFUNDIDADE.ToString(), 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"PROFUNDIDADE\" inválida - Nome reservado para token de Profundidade.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComProfundidadeInicialComMsgCustomizada()
        {
            _variáveis.AddVar(TokensEnum.PROFUNDIDADE_INICIAL.ToString(), 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"PROFUNDIDADE_INICIAL\" inválida - Nome reservado para token de Profundidade inicial.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComRhobInicialComMsgCustomizada()
        {
            _variáveis.AddVar(TokensEnum.RHOB_INICIAL.ToString(), 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"RHOB_INICIAL\" inválida - Nome reservado para token de RHOB inicial.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComStepComMsgCustomizada()
        {
            _variáveis.AddVar(TokensEnum.STEP.ToString(), 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"STEP\" inválida - Nome reservado para token de Diferença entre profundidades subsequentes.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComGrupoLitológico()
        {
            foreach (var grupoLitológico in GetGruposLitológicos())
            {
                _variáveis.AddVar(grupoLitológico, 1);
                var result = _sut.Validate(_variáveis);
                Check.That(result.IsValid).IsFalse();
                _variáveis.Clear();
            }
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComGrupoLitológicoComMsgCustomizada()
        {
            _variáveis.AddVar("Carbonatos", 1);
            var result = _sut.Validate(_variáveis);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Variável \"Carbonatos\" inválida - Nome reservado para Grupo Litológico");
        }


        private List<string> GetTokens()
        {
            return Enum.GetNames(typeof(TokensEnum)).ToList();
        }

        private List<string> GetTiposPerfil()
        {
            return TiposVálidos.Perfis.ToList();
        }

        private List<string> GetGruposLitológicos()
        {
            return GrupoLitologico.GetNames();
        }
    }
}
