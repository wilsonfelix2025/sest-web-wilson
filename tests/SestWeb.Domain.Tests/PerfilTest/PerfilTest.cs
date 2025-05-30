using System;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Tests.PerfilTest
{
    [TestFixture]
    public class PerfilTest
    {
        [TestCase("DTC")]
        [TestCase("GSOBR")]
        [TestCase("ExpoenteD")]
        [TestCase("GFRAT_σh")]
        [TestCase("THORmax")]
        [TestCase("RET")]
        public void DeveRetornarTipoDadoUmMnemônico(string mnemônico)
        {
            var tipoPerfil = TiposPerfil.GeTipoPerfil(mnemônico);

            Check.That(tipoPerfil.Mnemônico.ToUpperInvariant()).IsEqualTo(mnemônico.ToUpperInvariant());
        }

        [TestCase("XXX")]
        [TestCase("aaa")]
        [TestCase("bbb")]
        public void DeveLançarExceçãoCasoMnemônicoNãoExista(string mnemônico)
        {
            Check.ThatCode(() => { TiposPerfil.GeTipoPerfil(mnemônico); }).Throws<ArgumentException>().WithMessage($"Mnemônico inválido - {mnemônico}");
        }
    }
}