using System;

namespace CoreMVVM.IOC.Builder
{
    public interface IRegistrationBuilder
    {
        /// <summary>
        /// Gets the type being registrated.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets a value indicating if the new registrations are a single singleton.
        /// </summary>
        bool IsSingleton { get; }

        /// <summary>
        /// Gets the factory being registered. May be null.
        /// </summary>
        Func<IContainer, object> Factory { get; }

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
    }
}