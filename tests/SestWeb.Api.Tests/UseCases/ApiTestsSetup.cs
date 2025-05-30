using NUnit.Framework;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Api.Tests.UseCases
{
    [SetUpFixture]
    public class ApiTestsSetup
    {
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            PerfilBase.Map();
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            // algo global para executar após o teste
        }
    }
}
