using Autofac;

namespace SestWeb.Api
{
    /// <summary>
    /// Modulo do autofac para configurar o reconhecimento das classes pelo container
    /// </summary>
    public class Module : Autofac.Module
    {
        /// <summary>
        /// Realiza o carregamento das classes que serão gerencidadas pelo container
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            // Registra todos os tipos presentes no projeto SestWeb.Api
            builder.RegisterAssemblyTypes(typeof(Startup).Assembly)
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}
