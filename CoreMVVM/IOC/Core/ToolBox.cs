using CoreMVVM.IOC.Builder;
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
        private readonly Dictionary<Type, IRegistration> _registrations = new Dictionary<Type, IRegistration>();

        private readonly Dictionary<Type, ConstructorInfo> _constructors = new Dictionary<Type, ConstructorInfo>();
        private readonly Dictionary<ConstructorInfo, ParameterInfo[]> _parameters = new Dictionary<ConstructorInfo, ParameterInfo[]>();

        public ToolBox()
        {
        }

        #region Methods

        public bool TryGetRegistration(Type type, out IRegistration registration)
        {
            return _registrations.TryGetValue(type, out registration);
        }

        public IRegistration AddRegistration(Type componentType, Type serviceType, ComponentScope scope)
        {
            if (!serviceType.IsAssignableFrom(componentType))
                throw new IncompatibleTypeException($"Component type '{componentType}' does not inherit from or implement type '{serviceType}'.");

            var registration = _registrations.Values.FirstOrDefault(r => r.Type == componentType && r.Scope == scope);
            if (registration is null)
            {
                registration = new Registration(componentType, scope);
            }

            _registrations[serviceType] = registration;
            return registration;
        }

        public IRegistration AddRegistration(Type componentType, Type serviceType, ComponentScope scope, Func<ILifetimeScope, object> factory)
        {
            if (!serviceType.IsAssignableFrom(componentType))
                throw new IncompatibleTypeException($"Component type '{componentType}' does not inherit from or implement type '{serviceType}'.");

            var registration = _registrations.Values.FirstOrDefault(r => r.Type == componentType && r.Scope == scope);
            if (registration is null)
            {
                registration = new Registration(componentType, scope);
            }

            registration.Factory = factory;
            _registrations[serviceType] = registration;

            return registration;
        }

        public ConstructorInfo GetConstructor(Type type)
        {
            if (_constructors.TryGetValue(type, out var constructor))
            {
                return constructor;
            }

            return AddConstructor(type);
        }

        public ParameterInfo[] GetParameterInfo(ConstructorInfo constructor) => _parameters[constructor];

        #endregion Methods

        #region Private methods

        private ConstructorInfo AddConstructor(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            if (constructors.Length == 0)
                throw new ResolveException($"Type '{type}' has no accessible constructors.");

            var constructor = constructors
                .OrderByDescending(c => c.GetParameters().Length)
                .First();

            _constructors[type] = constructor;
            AddParameterInfo(constructor);

            return constructor;
        }

        private ParameterInfo[] AddParameterInfo(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();
            ValidateParameters(constructor.DeclaringType, parameters);

            _parameters[constructor] = parameters;

            return parameters;
        }

        private static void ValidateParameters(Type type, ParameterInfo[] parameters)
        {
            List<ResolveException> exceptions = null;
            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType.IsValueType)
                {
                    addException($"Parameter of type '{parameter.ParameterType}' is invalid.");
                }
            }

            if (exceptions != null)
            {
                var aggregateException = new AggregateException(exceptions);
                throw new ResolveException($"One or more parameters in the constructor of type '{type}' were invalid.", aggregateException);
            }

            void addException(string message)
            {
                if (exceptions == null)
                    exceptions = new List<ResolveException>();

                exceptions.Add(new ResolveException(message));
            }
        }

        #endregion Private methods
    }
}