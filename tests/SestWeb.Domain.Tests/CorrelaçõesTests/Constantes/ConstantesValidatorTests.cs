using System;
using System.Collections.Generic;
using System.Linq;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.ConstantesCorrelação.Validator;
using SestWeb.Domain.Entities.Correlações.ExpressãoCorrelação.TiposVálidosCorrelação;
using SestWeb.Domain.Entities.Correlações.TokensCorrelação;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.Constantes
{
    [TestFixture]
    public class ConstantesValidatorTests
    {
        private ConstantesValidator _sut;
        private ConstantesTestHelper _constantes;

        [SetUp]
        public void Setup()
        {
            _sut = new ConstantesValidator();
            _constantes = new ConstantesTestHelper(string.Empty);
        }

        [TearDown]
        public void TearDown()
        {
            _constantes?.Clear();
        }

        [Test]
        public void DeveResultarErroQuandoConstantesÉNull()
        {
            _constantes = null;
            var result = _sut.Validate(_constantes);
            Check.That(result.IsValid).IsFalse();
        }

        [Test]
        public void ErrorsCountDeveSer1QuandoConstantesÉNull()
        {
            _constantes = null;
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors.Count).IsEqualTo(1);
        }

        [Test]
        public void DeveApresentarMensagemConstantesÉNull()
        {
            _constantes = null;
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo($"{typeof(Entities.Correlações.ConstantesCorrelação.Constantes).Name} não pode ser null.");
        }

        [Test]
        public void DeveInvalidarConstanteComNomeNumérico()
        {
            _constantes.AddConst("1", 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.IsValid).IsFalse();
        }

        [Test]
        public void DeveInvalidarConstanteComNomeNuméricoComMsgCustomizada()
        {
            _constantes.AddConst("1", 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"1\" inválida - Não é permitido nomes de constantes com formato numérico!");
        }

        [Test]
        public void DeveInvalidarConstanteComNomecontendoCaracteresEspeciais()
        {
            _constantes.AddConst("@", 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo($"Constante \"{"@"}\" inválida - Nome contendo caracteres não permitidos!");
        }

        [Test]
        public void DeveReconhecerConstantesComGrandePrecisão()
        {
            _constantes.AddConst("a", 0.0000000000000000000000000000001);
            var result = _sut.Validate(_constantes);
            Check.That(result.IsValid).IsTrue();
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComTipoDePerfil()
        {
            foreach (var perfil in GetTiposPerfil())
            {
                _constantes.AddConst(perfil, 1);
                var result = _sut.Validate(_constantes);
                Check.That(result.IsValid).IsFalse();
                _constantes.Clear();
            }
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComTipoDePerfilComMsgCustomizada()
        {
            _constantes.AddConst("DTS", 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"DTS\" inválida - Nome reservado para entrada de perfis!");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComTokenDaCorrelação()
        {
            foreach (var token in GetTokens())
            {
                _constantes.AddConst(token, 1);
                var result = _sut.Validate(_constantes);
                Check.That(result.IsValid).IsFalse();
                _constantes.Clear();
            }
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComTokenDaCorrelaçãoComMsgCustomizada()
        {
            _constantes.AddConst("MESA_ROTATIVA", 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"MESA_ROTATIVA\" inválida - Nome reservado para token de Mesa Rotativa.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComAlturaAntepoçoComMsgCustomizada()
        {
            _constantes.AddConst(TokensEnum.ALTURA_ANTEPOCO.ToString(), 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"ALTURA_ANTEPOCO\" inválida - Nome reservado para token de Altura do Antepoço.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComCategoriaPoçoComMsgCustomizada()
        {
            _constantes.AddConst(TokensEnum.CATEGORIA_POCO.ToString(), 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"CATEGORIA_POCO\" inválida - Nome reservado para token de Categoria do Poço.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComDensidadeAguaMarComMsgCustomizada()
        {
            _constantes.AddConst(TokensEnum.DENSIDADE_AGUA_MAR.ToString(), 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"DENSIDADE_AGUA_MAR\" inválida - Nome reservado para token de Densidade da água do mar.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComGrupoLitologicoComMsgCustomizada()
        {
            _constantes.AddConst(TokensEnum.GRUPO_LITOLOGICO.ToString(), 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"GRUPO_LITOLOGICO\" inválida - Nome reservado para token de Grupo Litológico.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComLaminaDaguaComMsgCustomizada()
        {
            _constantes.AddConst(TokensEnum.LAMINA_DAGUA.ToString(), 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"LAMINA_DAGUA\" inválida - Nome reservado para token de Lâmina d'água.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComPoçoOffshoreComMsgCustomizada()
        {
            _constantes.AddConst(TokensEnum.OFFSHORE.ToString(), 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"OFFSHORE\" inválida - Nome reservado para token de Poço Offshore.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComPoçoOnshoreComMsgCustomizada()
        {
            _constantes.AddConst(TokensEnum.ONSHORE.ToString(), 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"ONSHORE\" inválida - Nome reservado para token de Poço Onshore.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComProfundidadeComMsgCustomizada()
        {
            _constantes.AddConst(TokensEnum.PROFUNDIDADE.ToString(), 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"PROFUNDIDADE\" inválida - Nome reservado para token de Profundidade.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComProfundidadeInicialComMsgCustomizada()
        {
            _constantes.AddConst(TokensEnum.PROFUNDIDADE_INICIAL.ToString(), 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"PROFUNDIDADE_INICIAL\" inválida - Nome reservado para token de Profundidade inicial.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComRhobInicialComMsgCustomizada()
        {
            _constantes.AddConst(TokensEnum.RHOB_INICIAL.ToString(), 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"RHOB_INICIAL\" inválida - Nome reservado para token de RHOB inicial.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComStepComMsgCustomizada()
        {
            _constantes.AddConst(TokensEnum.STEP.ToString(), 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"STEP\" inválida - Nome reservado para token de Diferença entre profundidades subsequentes.");
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComGrupoLitológico()
        {
            foreach (var grupoLitológico in GetGruposLitológicos())
            {
                _constantes.AddConst(grupoLitológico, 1);
                var result = _sut.Validate(_constantes);
                Check.That(result.IsValid).IsFalse();
                _constantes.Clear();
            }
        }

        [Test]
        public void DeveInvalidarQuandoNomeCoincideComGrupoLitológicoComMsgCustomizada()
        {
            _constantes.AddConst("Carbonatos", 1);
            var result = _sut.Validate(_constantes);
            Check.That(result.Errors[0].ErrorMessage).IsEqualTo("Constante \"Carbonatos\" inválida - Nome reservado para Grupo Litológico");
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
