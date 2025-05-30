using System;
using System.Collections.Generic;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Poço.EstratigrafiaDoPoço;

namespace SestWeb.Domain.Tests.EstratigrafiaTests
{
    [TestFixture]
    public class TipoEstratigrafiaTests
    {
        List<TipoEstratigrafia> _tipos;

        [SetUp]
        public void Setup()
        {
            _tipos = new List<TipoEstratigrafia>
            {
                TipoEstratigrafia.Cronoestratigráfica,
                TipoEstratigrafia.Formação,
                TipoEstratigrafia.Membro,
                TipoEstratigrafia.Camada,
                TipoEstratigrafia.Grupo,
                TipoEstratigrafia.Marco,
                TipoEstratigrafia.Indefinido,
                TipoEstratigrafia.Outros
            };
        }

        [Test]
        public void DeveObterTodosOsTiposDeEstratigrafia()
        {
            var result = TipoEstratigrafia.ListarTipos();

            Check.That(result).Equals(_tipos);
        }

        [Test]
        public void DeveObterTipoPorSigla()
        {
            var siglas = new List<string> { "CR", "FM", "MB", "CM", "GR", "MC", "IN", "OU" };

            Check.That(siglas.Count).IsEqualTo(_tipos.Count);

            for (var i = 0; i < _tipos.Count; i++)
            {
                var result = TipoEstratigrafia.ObterPeloTipo(siglas[i]);
                Check.That(result).IsEqualTo(_tipos[i]);
            }
        }

        [Test]
        public void DeveLançarArgumentNullExceptionSeTipoNulo()
        {
            const string siglainválida = null;

            Check.ThatCode(() => TipoEstratigrafia.ObterPeloTipo(siglainválida)).Throws<ArgumentNullException>();
        }

        [Test]
        public void DeveLançarInvalidOperationExceptionSeTipoNãoExiste()
        {
            const string siglainválida = "AA";

            Check.ThatCode(() => TipoEstratigrafia.ObterPeloTipo(siglainválida)).Throws<InvalidOperationException>();
        }
    }
}