using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using SestWeb.Domain.Tests.AutofacTests.Common.Types;

namespace SestWeb.Domain.Tests.AutofacTests.Common
{
    public static class ScopeExtensions
    {
        private static IList<IServiceWithType> FilterIgnored(this IEnumerable<IServiceWithType> services,
            TypesExpectedToBeIgnored ignoredAssemblies)
        {
            return services.Where(serviceWithType => ignoredAssemblies
                .All(ignored => ignored.ToString() != serviceWithType.ServiceType.Name)).ToList();
        }

        private static IList<IServiceWithType> FilterRegistered(this IEnumerable<IServiceWithType> services,
            TypesExpectedToBeRegistered registeredAssemblies)
        {
            return services.Where(serviceWithType => registeredAssemblies
                .All(registered => registered.ToString() == serviceWithType.ServiceType.Name)).ToList();
        }

        private static IEnumerable<IServiceWithType> FilterResolved(this IEnumerable<IServiceWithType> services,
            TypesExpectedToBeResolved resolvedAssemblies)
        {
            List<IServiceWithType> toBeResolvedServices = new List<IServiceWithType>();
            foreach (var resolvedAssembly in resolvedAssemblies)
            {
                var assemblyName = resolvedAssembly.First().ToString();
                foreach (var serviceWithType in services)
                {
                    var serviceName = serviceWithType.ServiceType.FullName;
                    if (serviceName.Equals(assemblyName) && !toBeResolvedServices.Contains(serviceWithType))
                        toBeResolvedServices.Add(serviceWithType);
                }
            }

            return toBeResolvedServices;
        }

        public static bool IsRegistered(this IEnumerable<IServiceWithType> services, TypesExpectedToBeRegistered registeredAssemblies)
        {
            foreach (var registeredAssembly in registeredAssemblies)
            {
                var assemblyName = registeredAssembly.First().ToString();
                if (!services.Any(serviceWithType => serviceWithType.ServiceType.FullName.Equals(assemblyName)))
                    return false;
            }

            return true;
        }

        public static IList<object> TryResolveAll(this ILifetimeScope scope, TypesExpectedToBeIgnored ignoredAssemblies)
        {
            var registrations = scope.ComponentRegistry.Registrations;
            var services = registrations.SelectMany(x => x.Services)//.Filter(ignoredAssemblies).ToList();
            .OfType<IServiceWithType>().FilterIgnored(ignoredAssemblies).ToList();

            foreach (var serviceWithType in services)
            {
                try
                {
                    scope.Resolve(serviceWithType.ServiceType);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return services.Select(x => x.ServiceType).Select(scope.Resolve).ToList();
        }

        public static IList<object> TryResolve(this ILifetimeScope scope, TypesExpectedToBeResolved resolvedAssemblies)
        {
            var registrations = scope.ComponentRegistry.Registrations;
            
            var services = registrations.SelectMany(x => x.Services)
                .OfType<IServiceWithType>().FilterResolved(resolvedAssemblies);

            foreach (var serviceWithType in services)
            {
                try
                {
                    scope.Resolve(serviceWithType.ServiceType);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return services.Select(x => x.ServiceType).Select(scope.Resolve).ToList();
        }
    }
}
