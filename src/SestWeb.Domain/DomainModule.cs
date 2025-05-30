using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core.Activators.Reflection;
using Module = Autofac.Module;

namespace SestWeb.Domain
{
    public class DomainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblyName = GetType().AssemblyQualifiedName;
            if (builder.Properties.ContainsKey(assemblyName))
            {
                return;
            }
            builder.Properties.Add(GetType().AssemblyQualifiedName, null);

            // Registra todos os tipos do assembly SestWeb.Domain
            builder.RegisterAssemblyTypes(typeof(DomainModule).Assembly)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

        }
    }

    public class NonPublicConstructorFinder : IConstructorFinder
    {
        public ConstructorInfo[] FindConstructors(Type targetType)
            => targetType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public class InternalConstructorFinder : IConstructorFinder
    {
        public ConstructorInfo[] FindConstructors(Type t) => t.GetTypeInfo().DeclaredConstructors.Where(c => c.IsPrivate & !c.IsPublic).ToArray();
    }
}
