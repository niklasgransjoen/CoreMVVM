using System;

namespace CoreMVVM.IOC
{
    /// <summary>
    /// Provides a default implementation to resolve an interface to when none are registered.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class FallbackImplementationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackImplementationAttribute"/> class.
        /// </summary>
        /// <param name="type">The type to fall back to.</param>
        public FallbackImplementationAttribute(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the type to fall back to.
        /// </summary>
        public Type Type { get; }
    }
}