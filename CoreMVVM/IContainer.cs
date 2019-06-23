using System;

namespace CoreMVVM
{
    /// <summary>
    /// Stores registrated components.
    /// </summary>
    public interface IContainer
    {
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