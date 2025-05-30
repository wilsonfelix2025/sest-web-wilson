using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.LitologiaDoPoco;

namespace SestWeb.Domain.Tests.LitologiaTests.GrupoLitologicoTests
{
    public class GrupoLitológicoTests
    {
        [Test]
        public void DeveObterCorretamenteOsTiposDeGrupoLitológico()
        {
            var grupos = GrupoLitologico.GetNames();
            Check.That(grupos).IsNotNull();
            Check.That(grupos.Count).IsEqualTo(9);
            Check.That(grupos).ContainsNoDuplicateItem();
            Check.That(grupos).Contains("Carbonatos");
            Check.That(grupos).Contains("Detríticas");
            Check.That(grupos).Contains("Arenitos");
            Check.That(grupos).Contains("Argilosas");
            Check.That(grupos).Contains("Ígneas");
            Check.That(grupos).Contains("Metamórficas");
            Check.That(grupos).Contains("Evaporitos");
            Check.That(grupos).Contains("Outros");
            Check.That(grupos).Contains("NãoIdentificado");
        }
    }
}
