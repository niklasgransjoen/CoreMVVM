using System;
using System.Linq;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// For performing component/service registration.
    /// </summary>
    public class RegistrationBuilder
    {
        private readonly RegistrationCollection _registrations;
        private readonly Type _type;
        private readonly bool _isSingleton;

        internal RegistrationBuilder(RegistrationCollection registrations, Type type)
            : this(registrations, type, isSingleton: false)
        {
        }

        internal RegistrationBuilder(RegistrationCollection registrations, Type type, bool isSingleton)
        {
            _registrations = registrations;
            _type = type;
            _isSingleton = isSingleton;
        }

        #region Methods

        /// <summary>
        /// Registers the current type as a component of T.
        /// </summary>
        public RegistrationBuilder As<T>()
        {
            if (!_isSingleton)
                Register(typeof(T));
            else
                RegisterSingleton(typeof(T));

            return this;
        }

        /// <summary>
        /// Registers the current type as a component of itself.
        /// </summary>
        public RegistrationBuilder AsSelf()
        {
            if (!_isSingleton)
                Register(_type);
            else
                RegisterSingleton(_type);

            return this;
        }

        #endregion Methods

        #region Private methods

        private void Register(Type type)
        {
            _registrations[type] = new Registration(_type);
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
                _registrations[type] = new Registration(_type)
                {
                    IsSingleton = true,
                };
            }
        }

        #endregion Private methods
    }
}