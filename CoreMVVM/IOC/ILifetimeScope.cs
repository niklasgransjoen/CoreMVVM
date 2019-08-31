using System;

namespace CoreMVVM.IOC
{
    public interface ILifetimeScope : IDisposable
    {
        /// <summary>
        /// Gets a value indicating if this lifetime scope is disposed.
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
        T Resolve<T>();

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <param name="type">The type to get an instance for.</param>
        object Resolve(Type type);
    }
}