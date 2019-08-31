using CoreMVVM.Extentions;
using CoreMVVM.IOC.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreMVVM.IOC.Core
{
    /// <summary>
    /// Implementation of <see cref="IContainer"/>.
    /// </summary>
    internal class Container : IContainer
    {
        private readonly IReadOnlyDictionary<Type, IRegistration> _registeredTypes;

        /// <summary>
        /// Creates a new container.
        /// </summary>
        /// <param name="registeredTypes">The registered types of this container.</param>
        public Container(IReadOnlyDictionary<Type, IRegistration> registeredTypes)
        {
            _registeredTypes = registeredTypes;
        }

        #region Methods

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <typeparam name="T">The type to get an instance for.</typeparam>
        /// <exception cref="ResolveUnregisteredInterfaceException">type of the registration of type is an interface.</exception>
        /// <exception cref="ResolveConstructionException">Fails to construct type or one of its arguments.</exception>
        public T Resolve<T>() => (T)Resolve(typeof(T));

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <param name="type">The type to get an instance for.</param>
        /// <exception cref="ResolveUnregisteredInterfaceException">type of the registration of type is an interface.</exception>
        /// <exception cref="ResolveConstructionException">Fails to construct type or one of its arguments.</exception>
        public object Resolve(Type type)
        {
            bool isRegistered = _registeredTypes.TryGetValue(type, out IRegistration registration);
            if (isRegistered)
            {
                if (registration.IsSingleton && registration.LastCreatedInstance != null)
                    return registration.LastCreatedInstance;

                object instance;
                if (registration.Factory != null)
                    instance = registration.Factory(this);
                else
                    instance = ConstructType(registration.Type);

                registration.LastCreatedInstance = instance;
                return instance;
            }

            return ConstructType(type);
        }

        /// <summary>
        /// Constructs an instance of the given type, using the constructor with the most parameters.
        /// </summary>
        /// <exception cref="ResolveConstructionException">Fails to construct type.</exception>
        private object ConstructType(Type type)
        {
            if (type.IsInterface)
                throw new ResolveUnregisteredInterfaceException($"Expected class or struct, recieved interface '{type}'.");

            try
            {
                ConstructorInfo[] constructors = type.GetConstructors();
                if (constructors.Length == 0)
                    return type.GetDefault();

                ConstructorInfo constructor = constructors
                    .OrderByDescending(c => c.GetParameters().Length)
                    .First();

                object[] args = constructor.GetParameters()
                                           .Select(param => Resolve(param.ParameterType))
                                           .ToArray();

                return constructor.Invoke(args);
            }
            catch (Exception e)
            {
                string message = $"Failed to construct instance of type '{type}'.";

                Resolve<ILogger>().Exception(message, e);
                throw new ResolveConstructionException(message, e);
            }
        }

        #endregion Methods
    }
}