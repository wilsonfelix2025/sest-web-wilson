using Autofac;
using SestWeb.Infra.Services;

namespace SestWeb.Infra.MongoDataAccess
{
    public class Module : Autofac.Module
    {
        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Context>()
                .As<Context>()
                .WithParameter("connectionString", ConnectionString)
                .WithParameter("databaseName", DatabaseName)
                .SingleInstance();

            builder.RegisterType<EmailService>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<FileService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();


            // Registra todos os tipos contidos no namespace SestWeb.Infra.MongoDataAccess
            builder.RegisterAssemblyTypes(typeof(Module).Assembly)
                .Where(type => type.Namespace.Contains("MongoDataAccess"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
