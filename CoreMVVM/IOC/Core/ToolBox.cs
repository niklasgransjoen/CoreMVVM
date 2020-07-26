﻿using CoreMVVM.IOC.Builder;
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
        private readonly Dictionary<(Type implementationType, ComponentScope scope), IRegistration> _registrationMap = new Dictionary<(Type implementationType, ComponentScope scope), IRegistration>();
        private readonly Dictionary<Type, List<IRegistration>> _registrations = new Dictionary<Type, List<IRegistration>>();

        private readonly Dictionary<Type, ConstructorInfo> _constructors = new Dictionary<Type, ConstructorInfo>();
        private readonly Dictionary<ConstructorInfo, ParameterInfo[]> _parameters = new Dictionary<ConstructorInfo, ParameterInfo[]>();

        public ToolBox()
        {
        }

        #region Methods

        public bool TryGetRegistration(Type serviceType, out IRegistration registration)
        {
            bool result = _registrations.TryGetValue(serviceType, out var registrations);
            if (result)
            {
                registration = registrations[registrations.Count - 1];
                return true;
            }

            registration = null;
            return false;
        }

#if NET45
        private static readonly IRegistration[] _emptyRegistrations = new IRegistration[0];
#endif

        public IReadOnlyList<IRegistration> GetRegistrations(Type serviceType)
        {
            if (_registrations.TryGetValue(serviceType, out var registrations))
                return registrations;

#if NET45
            return _emptyRegistrations;
#else
            return Array.Empty<IRegistration>();
#endif
        }

        public IRegistration AddRegistration(Type componentType, Type serviceType, ComponentScope scope)
        {
            if (!serviceType.IsAssignableFrom(componentType))
                throw new IncompatibleTypeException($"Component type '{componentType}' does not inherit from or implement type '{serviceType}'.");

            if (!_registrationMap.TryGetValue((componentType, scope), out var registration))
            {
                registration = new Registration(componentType, scope);
                _registrationMap.Add((componentType, scope), registration);
            }

            if (!_registrations.TryGetValue(serviceType, out var serviceRegistrations))
            {
                serviceRegistrations = new List<IRegistration>();
                _registrations.Add(serviceType, serviceRegistrations);
            }
            serviceRegistrations.Add(registration);

            return registration;
        }

        public IRegistration AddRegistration(Type componentType, Type serviceType, ComponentScope scope, Func<ILifetimeScope, object> factory)
        {
            var registration = AddRegistration(componentType, serviceType, scope);
            registration.Factory = factory;

            return registration;
        }

        public ConstructorInfo GetConstructor(Type type, bool validateParameters = true, Func<IEnumerable<ConstructorInfo>, ConstructorInfo> constructorSelector = null)
        {
            if (_constructors.TryGetValue(type, out var constructor))
            {
                return constructor;
            }

            return AddConstructor(type, validateParameters, constructorSelector ?? DefaultConstructorSelector);
        }

        public ParameterInfo[] GetParameterInfo(ConstructorInfo constructor) => _parameters[constructor];

        #endregion Methods

        #region Private methods

        private ConstructorInfo AddConstructor(Type type, bool validateParameters, Func<IEnumerable<ConstructorInfo>, ConstructorInfo> constructorSelector)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            if (constructors.Length == 0)
                throw new ResolveException($"Type '{type}' has no accessible constructors.");

            var constructor = constructorSelector(constructors);
            if (constructor is null)
                throw new ResolveException($"Type '{type}' has no valid constructor.");

            _constructors[type] = constructor;
            AddParameterInfo(constructor, validateParameters);

            return constructor;
        }

        private ParameterInfo[] AddParameterInfo(ConstructorInfo constructor, bool validateParameters)
        {
            var parameters = constructor.GetParameters();
            if (validateParameters)
            {
                ValidateParameters(constructor.DeclaringType, parameters);
            }

            _parameters[constructor] = parameters;

            return parameters;
        }

        #endregion Private methods

        #region Utilities

        private static ConstructorInfo DefaultConstructorSelector(IEnumerable<ConstructorInfo> constructors)
        {
            return constructors.OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();
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

        #endregion Utilities
    }
}