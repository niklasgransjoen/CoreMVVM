using System;
using System.Collections.Immutable;
using System.Linq;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// For performing component/service registration.
    /// </summary>
    public class RegistrationBuilder
    {
        private readonly RegistrationCollection _registrations;

        internal RegistrationBuilder(RegistrationCollection registrations, Type type)
            : this(registrations, type, isSingleton: false)
        {
        }

        internal RegistrationBuilder(RegistrationCollection registrations, Type type, bool isSingleton)
        {
            _registrations = registrations;
            Type = type;
            IsSingleton = isSingleton;
        }

        #region Properties

        /// <summary>
        /// Gets a collection of the registrations of this registration builder.
        /// </summary>
        public ImmutableDictionary<Type, Registration> Registrations => _registrations.ToImmutableDictionary();

        /// <summary>
        /// Gets the type being registrated.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets a value indicating if the new registrations are a single singleton.
        /// </summary>
        public bool IsSingleton { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Registers <see cref="Type"/> as a component of a given type.
        /// </summary>
        /// <typeparam name="T">The type to register <see cref="Type"/> as a component of.</typeparam>
        public RegistrationBuilder As<T>() => As(typeof(T));

        /// <summary>
        /// Registers <see cref="Type"/> as a component of a given type.
        /// </summary>
        /// <param name="type">The type to register <see cref="Type"/> as a component of.</param>
        public RegistrationBuilder As(Type type)
        {
            if (!IsSingleton)
                Register(type);
            else
                RegisterSingleton(type);

            return this;
        }

        /// <summary>
        /// Registers <see cref="Type"/> as a component of itself.
        /// </summary>
        public RegistrationBuilder AsSelf()
        {
            if (!IsSingleton)
                Register(Type);
            else
                RegisterSingleton(Type);

            return this;
        }

        #endregion Methods

        #region Private methods

        private void Register(Type type)
        {
            _registrations[type] = new Registration(Type);
        }

        private void RegisterSingleton(Type type)
        {
            // Check if type has been registered as a singleton already.
            // All Singleton registrations of a type must share registrations.
            Registration registration = _registrations.Values.FirstOrDefault(r => r.IsSingleton && r.Type == type);
            if (registration != null)
            {
                // Copy instance from previous registration.
                _registrations[type] = registration;
            }
            else
            {
                // Not registered before: create new registration.
                _registrations[type] = new Registration(Type)
                {
                    IsSingleton = true,
                };
            }
        }

        #endregion Private methods
    }
}