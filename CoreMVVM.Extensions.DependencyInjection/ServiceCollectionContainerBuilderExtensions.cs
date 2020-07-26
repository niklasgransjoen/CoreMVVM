using CoreMVVM.IOC;
using CoreMVVM.IOC.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CoreMVVM.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionContainerBuilderExtensions
    {
        /// <summary>
        /// Creates an IServiceProvider, using CoreMVVM's IOC.
        /// </summary>
        public static IServiceProvider BuildCoreMVVMContainer(this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            var builder = new ContainerBuilder();

            // For handling scoping.
            builder.RegisterSingleton<ServiceScopeFactory>()
                    .AsSelf()
                    .As<IServiceScopeFactory>();

            foreach (var service in services)
            {
                var componentScope = GetComponentScope(service.Lifetime);

                if (service.ImplementationFactory != null)
                {
                    var implementationType = service.ImplementationFactory.GetType().GetGenericArguments()[1];
                    builder.Register(implementationType, componentScope, service.ImplementationFactory).As(service.ServiceType);
                }
                else if (service.ImplementationInstance != null)
                {
                    builder.Register(service.ImplementationInstance.GetType(), componentScope, c => service.ImplementationInstance).As(service.ServiceType);
                }
                else
                {
                    builder.Register(service.ImplementationType, componentScope).As(service.ServiceType);
                }
            }

            // Return the factory's provider, as it supports scoping.
            return builder.Build()
                .GetRequiredService<ServiceScopeFactory>()
                .ServiceProvider;
        }

        private static ComponentScope GetComponentScope(ServiceLifetime serviceLifetime) => serviceLifetime switch
        {
            ServiceLifetime.Singleton => ComponentScope.Singleton,
            ServiceLifetime.Scoped => ComponentScope.LifetimeScope,
            _ => ComponentScope.Transient,
        };
    }
}