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

        public RegistrationBuilder(
            RegistrationCollection registrations,
            Type type,
            ComponentScope scope)
        {
            _registrations = registrations;

            Type = type;
            Scope = scope;
        }

        public RegistrationBuilder(
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

        #endregion Constructors

        #region Properties

        public Type Type { get; }

        public ComponentScope Scope { get; }

        public Func<ILifetimeScope, object> Factory { get; }

        #endregion Properties

        #region Methods

        public IRegistrationBuilder As(Type type)
        {
            if (!type.IsAssignableFrom(Type))
                throw new IncompatibleTypeException($"Component type '{Type}' does not inherit from or implement type '{type}'.");

            // If scope is limited, try copying the registration of an earlier registration of Type.
            if (Scope != ComponentScope.Transient)
            {
                if (TryCopyRegistration(type))
                    return this;
            }

            // Default to creating a new registration.
            _registrations[type] = new Registration(Type)
            {
                Scope = Scope,
                Factory = Factory,
            };

            return this;
        }

        #endregion Methods

        #region Private methods

        private bool TryCopyRegistration(Type type)
        {
            // Check if type has been registered already.
            // All registrations of a type with scope limitations must share a registration.
            IRegistration registration = _registrations.Values.FirstOrDefault(r => r.Type == Type);
            if (registration is null)
                return false;

            // Copy instance from previous registration.
            _registrations[type] = registration;

            // Overwrite previous factory
            registration.Factory = Factory;

            return true;
        }

        #endregion Private methods
    }

    internal class RegistrationBuilder<T> : RegistrationBuilder, IRegistrationBuilder<T>
        where T : class
    {
        public RegistrationBuilder(
            RegistrationCollection registrations,
            ComponentScope scope) 
            : base(registrations, typeof(T), scope)
        {
        }

        public RegistrationBuilder(
            RegistrationCollection registrations,
            ComponentScope scope,
            Func<ILifetimeScope, T> factory) 
            : base(registrations, typeof(T), scope, factory)
        {
        }
    }
}