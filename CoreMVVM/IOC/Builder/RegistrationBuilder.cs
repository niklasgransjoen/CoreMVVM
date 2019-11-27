using CoreMVVM.IOC.Core;
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

        private RegistrationBuilder(
            RegistrationCollection registrations,
            Type type,
            ComponentScope scope)
        {
            _registrations = registrations;

            Type = type;
            Scope = scope;
        }

        private RegistrationBuilder(
            RegistrationCollection registrations,
            Type type,
            ComponentScope scope,
            Func<ILifetimeScope, object> factory)
        {
            _registrations = registrations;

            Type = type;
            Scope = scope;
            Factory = factory;
        }

        #region No scope

        internal static RegistrationBuilder Create(RegistrationCollection registrations, Type type)
        {
            return new RegistrationBuilder(
                registrations,
                type,
                ComponentScope.None);
        }

        internal static RegistrationBuilder Create<T>(RegistrationCollection registrations, Func<ILifetimeScope, T> factory) where T : class
        {
            return new RegistrationBuilder(
                registrations,
                typeof(T),
                ComponentScope.None,
                factory);
        }

        #endregion No scope

        #region Singleton

        internal static RegistrationBuilder CreateSingleton(RegistrationCollection registrations, Type type)
        {
            return new RegistrationBuilder(
                registrations,
                type,
                ComponentScope.Singleton);
        }

        internal static RegistrationBuilder CreateSingleton<T>(RegistrationCollection registrations, Func<ILifetimeScope, T> factory) where T : class
        {
            return new RegistrationBuilder(
                registrations,
                typeof(T),
                ComponentScope.Singleton,
                factory);
        }

        #endregion Singleton

        #region Lifetime scope

        internal static RegistrationBuilder CreateLifetimeScope(RegistrationCollection registrations, Type type)
        {
            return new RegistrationBuilder(
                registrations,
                type,
                ComponentScope.LifetimeScope);
        }

        internal static RegistrationBuilder CreateLifetimeScope<T>(RegistrationCollection registrations, Func<ILifetimeScope, T> factory) where T : class
        {
            return new RegistrationBuilder(
                registrations,
                typeof(T),
                ComponentScope.LifetimeScope,
                factory);
        }

        #endregion Lifetime scope

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the type being registrated.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the scope <see cref="Type"/> is being registered in.
        /// </summary>
        public ComponentScope Scope { get; }

        /// <summary>
        /// Gets the factory being registered. May be null.
        /// </summary>
        public Func<ILifetimeScope, object> Factory { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Registers <see cref="Type"/> as a component of a given type.
        /// </summary>
        /// <typeparam name="T">The type to register <see cref="Type"/> as a component of.</typeparam>
        /// <exception cref="IncompatibleTypeException">The component does not inherit from or implement T.</exception>
        public IRegistrationBuilder As<T>() => As(typeof(T));

        /// <summary>
        /// Registers <see cref="Type"/> as a component of a given type.
        /// </summary>
        /// <param name="type">The type to register <see cref="Type"/> as a component of.</param>
        /// <exception cref="IncompatibleTypeException">The component does not inherit from or implement type.</exception>
        public IRegistrationBuilder As(Type type)
        {
            if (!type.IsAssignableFrom(Type))
                throw new IncompatibleTypeException($"Component type '{Type}' does not inherit from or implement type '{type}'.");

            Register(type);

            return this;
        }

        /// <summary>
        /// Registers <see cref="Type"/> as a component of itself.
        /// </summary>
        public IRegistrationBuilder AsSelf()
        {
            Register(Type);

            return this;
        }

        #endregion Methods

        #region Private methods

        private void Register(Type type)
        {
            // If scope is limited, try copying the registration of an earlier registration of Type.
            if (Scope != ComponentScope.None)
            {
                bool result = TryCopyRegistration(type);
                if (result)
                    return;
            }

            // Default to creating a new registration.
            _registrations[type] = new Registration(Type)
            {
                Scope = Scope,
                Factory = Factory,
            };
        }

        private bool TryCopyRegistration(Type type)
        {
            // Check if type has been registered already.
            // All registrations of a type with scope limitations must share a registration.
            IRegistration registration = _registrations.Values.FirstOrDefault(r => r.Type == Type);
            if (registration == null)
                return false;

            // Copy instance from previous registration.
            _registrations[type] = registration;

            // Overwrite previous factory
            _registrations[type].Factory = Factory;

            return true;
        }

        #endregion Private methods
    }
}