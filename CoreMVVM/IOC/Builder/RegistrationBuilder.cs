using System;
using System.Linq;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// For performing component/service registration.
    /// </summary>
    internal class RegistrationBuilder : IRegistrationBuilder
    {
        private readonly RegistrationCollection _registrations;

        #region Constructors

        private RegistrationBuilder(RegistrationCollection registrations, Type type, bool isSingleton, Func<IContainer, object> factory)
        {
            _registrations = registrations;

            Type = type;
            IsSingleton = isSingleton;
            Factory = factory;
        }

        internal static RegistrationBuilder Create<T>(RegistrationCollection registrations)
        {
            return new RegistrationBuilder(registrations, typeof(T), isSingleton: false, factory: null);
        }

        internal static RegistrationBuilder Create(RegistrationCollection registrations, Type type)
        {
            return new RegistrationBuilder(registrations, type, isSingleton: false, factory: null);
        }

        internal static RegistrationBuilder CreateSingleton<T>(RegistrationCollection registrations)
        {
            return new RegistrationBuilder(registrations, typeof(T), isSingleton: true, factory: null);
        }

        internal static RegistrationBuilder CreateSingleton(RegistrationCollection registrations, Type type)
        {
            return new RegistrationBuilder(registrations, type, isSingleton: true, factory: null);
        }

        internal static RegistrationBuilder CreateFactory<T>(RegistrationCollection registrations, Func<IContainer, T> factory)
        {
            return new RegistrationBuilder(registrations, typeof(T), isSingleton: false, c => factory(c));
        }

        internal static RegistrationBuilder CreateSingletonFactory<T>(RegistrationCollection registrations, Func<IContainer, T> factory)
        {
            return new RegistrationBuilder(registrations, typeof(T), isSingleton: true, c => factory(c));
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the type being registrated.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets a value indicating if the new registrations are a single singleton.
        /// </summary>
        public bool IsSingleton { get; }

        /// <summary>
        /// Gets the factory being registered. May be null.
        /// </summary>
        public Func<IContainer, object> Factory { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Registers <see cref="Type"/> as a component of a given type.
        /// </summary>
        /// <typeparam name="T">The type to register <see cref="Type"/> as a component of.</typeparam>
        public IRegistrationBuilder As<T>() => As(typeof(T));

        /// <summary>
        /// Registers <see cref="Type"/> as a component of a given type.
        /// </summary>
        /// <param name="type">The type to register <see cref="Type"/> as a component of.</param>
        public IRegistrationBuilder As(Type type)
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
        public IRegistrationBuilder AsSelf()
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
            _registrations[type] = new Registration(Type)
            {
                Factory = Factory,
            };
        }

        private void RegisterSingleton(Type type)
        {
            // Check if type has been registered as a singleton already.
            // All Singleton registrations of a type must share registrations.
            IRegistration registration = _registrations.Values.FirstOrDefault(r => r.IsSingleton && r.Type == Type);
            if (registration != null)
            {
                // Copy instance from previous registration.
                _registrations[type] = registration;

                // Overwrite previous factory
                _registrations[type].Factory = Factory;
            }
            else
            {
                // Not registered before: create new registration.
                _registrations[type] = new Registration(Type)
                {
                    IsSingleton = true,
                    Factory = Factory,
                };
            }
        }

        #endregion Private methods
    }
}