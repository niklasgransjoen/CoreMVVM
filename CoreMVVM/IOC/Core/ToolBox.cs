using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreMVVM.IOC.Core
{
    /// <summary>
    /// Container for cached mappings and constructors.
    /// </summary>
    internal sealed class ToolBox
    {
        private readonly RegistrationCollection _registrations = new RegistrationCollection();

        private readonly Dictionary<Type, ConstructorInfo> _constructors = new Dictionary<Type, ConstructorInfo>();
        private readonly Dictionary<ConstructorInfo, ParameterInfo[]> _parameters = new Dictionary<ConstructorInfo, ParameterInfo[]>();

        public ToolBox()
        {
        }

        public ToolBox(IReadOnlyDictionary<Type, IRegistration> registeredTypes)
        {
            foreach (var pair in registeredTypes)
                _registrations[pair.Key] = pair.Value;
        }

        #region Methods

        public bool TryGetRegistration(Type type, out IRegistration registration)
        {
            return _registrations.TryGetValue(type, out registration);
        }

        public IRegistration AddRegistration(Type component, Type type, ComponentScope scope)
        {
            // Make sure scopes components are only registered once.
            if (scope != ComponentScope.None)
            {
                var previousRegistration = _registrations.Values.SingleOrDefault(r => r.Type == component);
                if (previousRegistration != null)
                {
                    _registrations[type] = previousRegistration;
                    return previousRegistration;
                }
            }

            var registration = new Registration(component) { Scope = scope };
            _registrations[type] = registration;

            return registration;
        }

        public bool TryGetConstructor(Type type, out ConstructorInfo constructor)
        {
            return _constructors.TryGetValue(type, out constructor);
        }

        public void AddConstructor(Type type, ConstructorInfo constructor)
        {
            _constructors[type] = constructor;
        }

        public bool TryGetParameterInfo(ConstructorInfo constructor, out ParameterInfo[] parameters)
        {
            return _parameters.TryGetValue(constructor, out parameters);
        }

        public void AddParameterInfo(ConstructorInfo constructor)
        {
            _parameters[constructor] = constructor.GetParameters();
        }

        #endregion Methods
    }
}