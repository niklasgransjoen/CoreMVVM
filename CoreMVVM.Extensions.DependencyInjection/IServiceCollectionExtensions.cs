using CoreMVVM.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Creates an IServiceProvider, using CoreMVVM's IOC.
        /// </summary>
        public static IServiceProvider BuildCoreMVVMContainer(this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            var builder = new ContainerBuilder();

            builder.CopyFromServiceCollection(services);

            // For handling scoping.
            builder.RegisterSingleton<CoreMVVMServiceScopeFactory>()
                .AsSelf()
                .As<IServiceScopeFactory>();

            // Return the factory's provider, as it supports scoping.
            return builder.Build()
                .GetRequiredService<CoreMVVMServiceScopeFactory>()
                .ServiceProvider;
        }
    }
}