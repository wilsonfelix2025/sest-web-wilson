using Autofac;

namespace SestWeb.Infra.Services
{
    class PocoWebModule: Module
    {
        public string PocoWebUrl { get; private set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PocoWebService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .WithParameter("poçoWebUrl", PocoWebUrl);

            // Registra todos os tipos contidos no namespace SestWeb.Infra.MongoDataAccess
            builder.RegisterAssemblyTypes(typeof(PocoWebModule).Assembly)
                .Where(type => type.Namespace.Contains("Services"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
