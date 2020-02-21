using System;

namespace CoreMVVM.IOC
{
    public interface ILifetimeScope : IServiceProvider, IDisposable
    {
        /// <summary>
        /// Gets a value indicating if this container has been disposed.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Creates a new lifetime scope.
        /// </summary>
        /// <returns>A new lifetime scope.</returns>
        ILifetimeScope BeginLifetimeScope();

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <typeparam name="T">The type to get an instance for.</typeparam>
        /// <exception cref="ResolveUnregisteredInterfaceException">T is an unregistered or resolves to an interface.</exception>
        /// <exception cref="ResolveConstructionException">Fails to construct type or one of its arguments.</exception>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <param name="type">The type to get an instance for. Class or interface.</param>
        /// <exception cref="ResolveUnregisteredInterfaceException">type is an unregistered or resolves to an interface.</exception>
        /// <exception cref="ResolveConstructionException">Fails to construct type or one of its arguments.</exception>
        /// <exception cref="ArgumentException">type is value type.</exception>
        object Resolve(Type type);
    }
}