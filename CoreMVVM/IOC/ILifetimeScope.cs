using System;

namespace CoreMVVM.IOC
{
    /// <summary>
    /// An extension of <see cref="IServiceProvider"/>.
    /// </summary>
    /// <remarks>
    /// An instance implementing this interface represents a scoped service provider.
    /// </remarks>
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
    }
}