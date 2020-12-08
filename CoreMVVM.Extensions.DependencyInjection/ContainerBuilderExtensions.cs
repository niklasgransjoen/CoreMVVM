using Microsoft.Extensions.DependencyInjection;
using System;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// Extension methods for <see cref="ContainerBuilder"/>.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Copies all registrations from an <see cref="IServiceCollection"/> to the given <see cref="ContainerBuilder"/>.
        /// </summary>
        /// <returns>The same builder, for chaining.</returns>
        public static ContainerBuilder CopyFromServiceCollection(this ContainerBuilder builder, IServiceCollection services)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (services is null) throw new ArgumentNullException(nameof(services));

            foreach (var service in services)
            {
                var componentScope = getComponentScope(service.Lifetime);

                if (service.ImplementationFactory is not null)
                {
                    var implementationType = service.ImplementationFactory.GetType().GetGenericArguments()[1];
                    builder.Register(implementationType, componentScope, service.ImplementationFactory).As(service.ServiceType);
                }
                else if (service.ImplementationInstance is not null)
                {
                    builder.Register(service.ImplementationInstance.GetType(), componentScope, c => service.ImplementationInstance).As(service.ServiceType);
                }
                else if (service.ImplementationType is not null)
                {
                    builder.Register(service.ImplementationType, componentScope).As(service.ServiceType);
                }
                else
                {
                    builder.Register(service.ServiceType, componentScope).AsSelf();
                }

                static ComponentScope getComponentScope(ServiceLifetime serviceLifetime) => serviceLifetime switch
                {
                    ServiceLifetime.Singleton => ComponentScope.Singleton,
                    ServiceLifetime.Scoped => ComponentScope.LifetimeScope,
                    _ => ComponentScope.Transient,
                };
            }

            return builder;
        }
    }
}