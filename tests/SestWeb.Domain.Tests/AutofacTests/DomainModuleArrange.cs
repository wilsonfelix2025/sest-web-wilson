using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using NFluent;
using NUnit.Framework;
using SestWeb.Domain.Tests.AutofacTests.Common;
using SestWeb.Domain.Tests.AutofacTests.Common.Types;

namespace SestWeb.Domain.Tests.AutofacTests
{
    /// <summary>
    /// Arrange do Módulo do Domínio
    /// </summary>
	public class DomainModuleArrange : ContainerArrangeBase<DomainModule>
    {
        // tipos esperados a serem ignorados
        protected virtual TypesExpectedToBeIgnored ExpectedToBeIgnored => new TypesExpectedToBeIgnored(new List<Type>());

        // tipos esperados a serem registrados
        protected virtual TypesExpectedToBeRegistered ExpectedToBeRegistered => new TypesExpectedToBeRegistered(new List<Type>());

        // tipos esperados a serem resolvidos
        protected virtual TypesExpectedToBeResolved ExpectedToBeResolved => new TypesExpectedToBeResolved(new List<Type>());

        // módulos overridados
        protected override List<OverridesModule> Overrides => new List<OverridesModule> { new OverridesModule() };

        // Scope do container do Módulo do Domínio
        protected ILifetimeScope Scope;

        [SetUp]
        public void Setup() => Scope = Container.BeginLifetimeScope();

        [Test]
        public virtual void DeveDispararExcessãoCasoAlgumTipoRegistradoNãoSejaResolvido()
        {
            Assert.Throws<Autofac.Core.DependencyResolutionException>(() => Scope.TryResolveAll(ExpectedToBeIgnored));
        }

        [Test]
        public virtual void TodosTiposASeremRegistradosSãoRegistrados()
        {
            foreach (var expecterRegisteredType in ExpectedToBeRegistered)
            {
                var type = (Type)expecterRegisteredType.First();
                Check.That(Scope.IsRegistered(type)).IsTrue();
            }
        }

        [Test]
        public virtual void NãoDeveDispararExcessãoCasoOsTiposASeremResolvidosSãoResolvidos()
        {
            Assert.DoesNotThrow(() => Scope.TryResolve(ExpectedToBeResolved));
        }
    }
}
