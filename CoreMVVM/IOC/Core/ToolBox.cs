using CoreMVVM.IOC.Builder;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public bool TryGetRegistration(Type serviceType, [NotNullWhen(true)] out IRegistration? registration)
        {
            if (_registrations.TryGetValue(serviceType, out var registrations))
            {
                registration = registrations[registrations.Count - 1];
                return true;
            }

            if (serviceType.IsGenericType &&
                _registrations.TryGetValue(serviceType.GetGenericTypeDefinition(), out registrations))
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
            if (!serviceType.IsGenericType)
            {
                if (_registrations.TryGetValue(serviceType, out var registrations))
                    return registrations;
            }
            else
            {
                _registrations.TryGetValue(serviceType, out var registrations);
                _registrations.TryGetValue(serviceType.GetGenericTypeDefinition(), out var genericRegistrations);

                if (registrations is null ^ genericRegistrations is null)
                {
                    return registrations ?? genericRegistrations!;
                }
                else if (registrations != null && genericRegistrations != null)
                {
                    var result = new List<IRegistration>(registrations.Count + genericRegistrations.Count);
                    result.AddRange(registrations);
                    result.AddRange(genericRegistrations);

                    return result;
                }
            }

#if NET45
            return _emptyRegistrations;
#else
            return Array.Empty<IRegistration>();
#endif
        }

        public IRegistration AddRegistration(Type componentType, Type serviceType, ComponentScope scope)
        {
            if (serviceType.IsGenericTypeDefinition && componentType.IsGenericTypeDefinition)
            {
                var concreteServiceType = componentType.GetGenericBaseType(serviceType);
                if (concreteServiceType is null)
                    throw new IncompatibleGenericTypeDefinitionException($"Component '{componentType}' is not compatible with service '{serviceType}'.");

                var serviceGenericArguments = concreteServiceType.GetGenericArguments();
                var componentGenericArguments = componentType.GetGenericArguments();

                if (serviceGenericArguments.Length != componentGenericArguments.Length)
                    throw new IncompatibleGenericTypeDefinitionException($"Generic service '${serviceType}' does not have the same generic argument count as implementation '${componentType}'.");

                var incompatibleGenericArgument = serviceGenericArguments.Zip(componentGenericArguments, (t1, t2) => (t1, t2))
                    .Any(pair => pair.t1 != pair.t2);
                if (incompatibleGenericArgument)
                {
                    throw new IncompatibleGenericTypeDefinitionException($"Type arguments of generic service '${serviceType}' is not compatible with implementation '${componentType}'.");
                }
            }

            if (!serviceType.IsAssignableFromGeneric(componentType))
                throw new IncompatibleTypeException($"Component type '{componentType}' is not compatible with the service type '{serviceType}'.");

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

        public IRegistration AddRegistration(Type componentType, Type serviceType, ComponentScope scope, Func<ILifetimeScope, object>? factory)
        {
            var registration = AddRegistration(componentType, serviceType, scope);
            registration.Factory = factory;

            return registration;
        }

        public ConstructorInfo GetConstructor(Type type, bool validateParameters = true, Func<IEnumerable<ConstructorInfo>, ConstructorInfo?>? constructorSelector = null)
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

        private ConstructorInfo AddConstructor(Type type, bool validateParameters, Func<IEnumerable<ConstructorInfo>, ConstructorInfo?> constructorSelector)
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
            if (validateParameters)
            {
                ValidateParameters(constructor);
            }

            var parameters = constructor.GetParameters();
            _parameters[constructor] = parameters;

            return parameters;
        }

        #endregion Private methods

        #region Utilities

        private static ConstructorInfo? DefaultConstructorSelector(IEnumerable<ConstructorInfo> constructors)
        {
            return constructors.OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();
        }

        private static void ValidateParameters(MethodBase method)
        {
            List<ResolveException>? exceptions = null;
            foreach (var parameter in method.GetParameters())
            {
                if (parameter.ParameterType.IsValueType)
                {
                    addException($"Parameter of type '{parameter.ParameterType}' is invalid.");
                }
            }

            if (exceptions != null)
            {
                var aggregateException = new AggregateException(exceptions);
                throw new ResolveException($"One or more parameters in the constructor of type '{method.DeclaringType}' were invalid.", aggregateException);
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