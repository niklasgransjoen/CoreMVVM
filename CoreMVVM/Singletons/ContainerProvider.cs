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

        private static readonly object _fallbackLock = new object();
        private static IContainer _fallbackContainer;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the container instance the provider defaults to when <see cref="Container"/> is null.
        /// </summary>
        public static IContainer FallbackContainer
        {
            get
            {
                lock (_fallbackLock)
                {
                    if (_fallbackContainer is null)
                    {
                        ContainerBuilder builder = new ContainerBuilder();
                        _fallbackContainer = builder.Build();
                    }

                    return _fallbackContainer;
                }
            }
        }

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
        public static T Resolve<T>() => ContainerOrFallback().Resolve<T>();

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <param name="type">The type to get an instance for.</param>
        public static object Resolve(Type type) => ContainerOrFallback().Resolve(type);

        #endregion ILifetimeScope

        #region Helpers

        private static IContainer ContainerOrFallback() => Container ?? FallbackContainer;

        #endregion Helpers
    }
}