using System.Collections.Generic;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using NUnit.Framework;

namespace SestWeb.Domain.Tests.AutofacTests.Common
{
	public abstract class ContainerArrangeBase<TModule> where TModule : IModule, new()
    {
        protected IContainer Container { get; private set; }

        protected abstract List<OverridesModule> Overrides { get; }

        [SetUp]
        public void TestBaseSetup()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<TModule>();

            foreach (var moduleOverrides in Overrides)
            {
                containerBuilder.RegisterModule(moduleOverrides);
            }

            Container = containerBuilder.Build(
                ContainerBuildOptions.IgnoreStartableComponents);
        }

        [TearDown]
        public void TestFixtureTearDown()
        {
            Container.Dispose();
        }
    }
}
