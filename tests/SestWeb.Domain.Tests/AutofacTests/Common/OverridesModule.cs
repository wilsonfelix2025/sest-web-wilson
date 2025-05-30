using Autofac;
using Moq;

namespace SestWeb.Domain.Tests.AutofacTests.Common
{
	public class OverridesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.Register(c =>
            {
                return new Mock<IComponentContext>(c);
            });
        }
    }
}
