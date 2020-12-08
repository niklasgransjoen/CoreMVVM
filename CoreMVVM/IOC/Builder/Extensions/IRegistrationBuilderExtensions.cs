using System;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// Extension methods for <see cref="IRegistrationBuilder"/>.
    /// </summary>
    public static class IRegistrationBuilderExtensions
    {
        /// <summary>
        /// Registers a type as a component of service <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The service type to register the componet as.</typeparam>
        /// <exception cref="IncompatibleTypeException">The component does not inherit from or implement T.</exception>
        public static IRegistrationBuilder As<T>(this IRegistrationBuilder registrationBuilder)
        {
            if (registrationBuilder is null)
                throw new ArgumentNullException(nameof(registrationBuilder));

            return registrationBuilder.As(typeof(T));
        }

        /// <summary>
        /// Registers a component.
        /// </summary>
        /// <exception cref="IncompatibleTypeException">The component does not inherit from or implement T.</exception>
        public static IRegistrationBuilder AsSelf(this IRegistrationBuilder registrationBuilder)
        {
            if (registrationBuilder is null)
                throw new ArgumentNullException(nameof(registrationBuilder));

            return registrationBuilder.As(registrationBuilder.Type);
        }
    }
}