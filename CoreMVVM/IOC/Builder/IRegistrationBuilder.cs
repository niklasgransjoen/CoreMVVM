using System;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// Service used for building registrations.
    /// </summary>
    public interface IRegistrationBuilder
    {
        #region Properties

        /// <summary>
        /// Gets the type being registrated.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets the scope <see cref="Type"/> is being registered in.
        /// </summary>
        ComponentScope Scope { get; }

        /// <summary>
        /// Gets the factory being registered. May be null.
        /// </summary>
        Func<ILifetimeScope, object>? Factory { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Registers <see cref="Type"/> as a component of a given type.
        /// </summary>
        /// <param name="serviceType">The service to register <see cref="Type"/> as a component of.</param>
        /// <exception cref="IncompatibleTypeException">The component does not inherit from or implement type.</exception>
        IRegistrationBuilder As(Type serviceType);

        #endregion Methods
    }

    /// <summary>
    /// Service used for building registrations.
    /// </summary>
    public interface IRegistrationBuilder<T> : IRegistrationBuilder
    {
    }
}