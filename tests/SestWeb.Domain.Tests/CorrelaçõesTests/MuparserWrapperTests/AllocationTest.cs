using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Entities.Correlações.Parsers.MuParserWrapper;

namespace SestWeb.Domain.Tests.CorrelaçõesTests.MuparserWrapperTests
{
    [TestFixture]
    public class AllocationTest
    {
        private Parser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new MuParserNetWrapper("AllocationTest").Parser;
        }

        [Test]
        public void TesteAlocaçãoDinâmica()
        {
            Assert.IsNotNull(_parser);
        }

        [Test]
        public void TesteDeVersãoDoMuparser()
        {
            var version = _parser.GetVersion();
            Check.That(version).Contains("2.2.6-Sest (20181004; 64BIT; RELEASE; ASCII");
        }
    }
}
