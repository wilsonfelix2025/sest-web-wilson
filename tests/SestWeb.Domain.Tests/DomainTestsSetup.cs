using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SestWeb.Domain.Entities.Perfis.Base;

namespace SestWeb.Domain.Tests
{
    [SetUpFixture]
    public class DomainTestsSetup
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
