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

            builder.CopyFromServiceCollection(services);

            // Return the factory's provider, as it supports scoping.
            return builder.Build()
                .GetRequiredService<ServiceScopeFactory>()
                .ServiceProvider;
        }
    }
}