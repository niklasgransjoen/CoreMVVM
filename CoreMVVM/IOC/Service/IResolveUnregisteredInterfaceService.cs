using System;

namespace CoreMVVM.IOC
{
    /// <summary>
    /// Implements functionality for handling attempts to resolve unregistered interfaces.
    /// </summary>
    public interface IResolveUnregisteredInterfaceService
    {
        /// <summary>
        /// Handles the resolving of an unregistered interface.
        /// </summary>
        void Handle(ResolveUnregisteredInterfaceContext context);
    }

    /// <summary>
    /// Context for handling the resolving of an unregistered interface.
    /// </summary>
    public sealed class ResolveUnregisteredInterfaceContext
    {
        #region Constructor

        internal ResolveUnregisteredInterfaceContext(Type type)
        {
            if (!type.IsInterface)
                throw new ArgumentException($"'{type}' is not an interface.", nameof(type));

            InterfaceType = type;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the interface type that failed to resolve.
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        /// Gets the type to resolve to. Is a non-abstract, non-static class that implements <see cref="InterfaceType"/>.
        /// </summary>
        public Type? InterfaceImplementationType { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value of <see cref="InterfaceImplementationType"/> should be cached.
        /// </summary>
        /// <remarks>This property has no effect when <see cref="InterfaceImplementationType"/> is not set.</remarks>
        public bool CacheImplementation { get; set; }

        /// <summary>
        /// Gets or sets the scope to use when caching <see cref="InterfaceImplementationType"/>.
        /// </summary>
        /// <remarks>This property has no effect when <see cref="CacheImplementation"/> is not set.</remarks>
        public ComponentScope CacheScope { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sets <see cref="InterfaceImplementationType"/>.
        /// </summary>
        /// <param name="interfaceImplementationType">The type to resolve to in place of <see cref="InterfaceType"/>. Must implement <see cref="InterfaceType"/>.</param>
        public void SetInterfaceImplementationType(Type interfaceImplementationType)
        {
            if (!InterfaceType.IsAssignableFrom(interfaceImplementationType))
                throw new ArgumentException($"Type '{interfaceImplementationType}' does not implement interface '{InterfaceType}'.", nameof(interfaceImplementationType));

            if (!interfaceImplementationType.IsClass || interfaceImplementationType.IsAbstract)
                throw new ArgumentException($"Type '{interfaceImplementationType}' must be a non-abstract, non-static class.", nameof(interfaceImplementationType));

            InterfaceImplementationType = interfaceImplementationType;
        }

        #endregion Methods
    }
}