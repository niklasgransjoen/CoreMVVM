using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreMVVM
{
    /// <summary>
    /// Implementation of <see cref="IContainer"/>.
    /// </summary>
    public class Container : IContainer
    {
        private readonly IReadOnlyDictionary<Type, Registration> registeredTypes;

        /// <summary>
        /// Creates a new container.
        /// </summary>
        /// <param name="registeredTypes">The registered types of this container.</param>
        internal Container(IReadOnlyDictionary<Type, Registration> registeredTypes)
        {
            this.registeredTypes = registeredTypes;
        }

        #region Methods

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <typeparam name="T">The type to get an instance for.</typeparam>
        public T Resolve<T>() => (T)Resolve(typeof(T));

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <param name="type">The type to get an instance for.</param>
        public object Resolve(Type type)
        {
            Type outputType;
            bool isRegistered = registeredTypes.TryGetValue(type, out Registration registration);
            if (isRegistered)
            {
                if (registration.IsSingleton && registration.LastCreatedInstance != null)
                    return registration.LastCreatedInstance;

                outputType = registration.Type;
            }
            else outputType = type;

            if (outputType.IsInterface)
                throw new ResolveUnregisteredInterfaceException($"Expected class or struct, recieved interface '{outputType}' instead.");

            var constructor = outputType.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .First();

            var args = constructor.GetParameters()
                .Select(param => Resolve(param.ParameterType))
                .ToArray();

            object instance = constructor.Invoke(args);
            if (isRegistered)
                registration.LastCreatedInstance = instance;

            return instance;
        }

        #endregion Methods
    }

    public class ResolveUnregisteredInterfaceException : Exception
    {
        public ResolveUnregisteredInterfaceException(string message) : base(message)
        {
        }
    }
}