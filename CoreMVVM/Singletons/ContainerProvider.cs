using CoreMVVM.IOC;
using CoreMVVM.IOC.Builder;
using System;

namespace CoreMVVM
{
    /// <summary>
    /// Singleton providing access to the root container of the application.
    /// </summary>
    public static class ContainerProvider
    {
        #region Fields

        private static readonly Lazy<IContainer> _fallbackContainer = new Lazy<IContainer>(() => new ContainerBuilder().Build());

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the container instance the provider defaults to when <see cref="Container"/> is null.
        /// </summary>
        public static IContainer FallbackContainer => _fallbackContainer.Value;

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="FallbackContainer"/> should be used when <see cref="IContainer"/> is uninitialized.
        /// <para>When false, <see cref="NotInitializedException"/> is thrown.</para>
        /// </summary>
        /// <value>Default is false.</value>
        public static bool UseFallbackContainer { get; set; } = false;

        /// <summary>
        /// Gets or sets the container of this provider. May be null.
        /// </summary>
        public static IContainer Container { get; set; }

        #endregion Properties

        #region ILifetimeScope

        /// <summary>
        /// Creates a new lifetime scope.
        /// </summary>
        /// <returns>A new lifetime scope.</returns>
        public static ILifetimeScope BeginLifetimeScope() => ContainerOrFallback().BeginLifetimeScope();

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <typeparam name="T">The type to get an instance for.</typeparam>
        /// <exception cref="ResolveUnregisteredServiceException">no service of type T exist.</exception>
        public static T ResolveRequiredService<T>() where T : class
        {
            return ContainerOrFallback().ResolveService<T>();
        }

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <param name="type">The type to get an instance for.</param>
        /// <exception cref="ResolveUnregisteredServiceException">no service of type type exist.</exception>
        public static object ResolveRequiredService(Type type) => ContainerOrFallback().ResolveService(type);

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <typeparam name="T">The type to get an instance for.</typeparam>
        public static T ResolveService<T>() where T : class
        {
            return ContainerOrFallback().ResolveService<T>();
        }

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <param name="type">The type to get an instance for.</param>
        public static object ResolveService(Type type) => ContainerOrFallback().ResolveService(type);

        #endregion ILifetimeScope

        #region Helpers

        private static IContainer ContainerOrFallback()
        {
            if (Container != null)
                return Container;

            if (UseFallbackContainer)
                return FallbackContainer;

            throw new NotInitializedException($"ContainerProvider cannot be used when uninitialized and the '{nameof(UseFallbackContainer)}' flag is not set.", nameof(Container));
        }

        #endregion Helpers
    }
}