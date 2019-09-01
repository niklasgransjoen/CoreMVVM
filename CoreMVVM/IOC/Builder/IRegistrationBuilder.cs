using System;

namespace CoreMVVM.IOC.Builder
{
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
        InstanceScope Scope { get; }

        /// <summary>
        /// Gets the factory being registered. May be null.
        /// </summary>
        Func<ILifetimeScope, object> Factory { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Registers <see cref="Type"/> as a component of a given type.
        /// </summary>
        /// <typeparam name="T">The type to register <see cref="Type"/> as a component of.</typeparam>
        IRegistrationBuilder As<T>();

        /// <summary>
        /// Registers <see cref="Type"/> as a component of a given type.
        /// </summary>
        /// <param name="type">The type to register <see cref="Type"/> as a component of.</param>
        IRegistrationBuilder As(Type type);

        /// <summary>
        /// Registers <see cref="Type"/> as a component of itself.
        /// </summary>
        IRegistrationBuilder AsSelf();

        #endregion Methods
    }
}