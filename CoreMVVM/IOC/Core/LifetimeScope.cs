using CoreMVVM.Extentions;
using CoreMVVM.IOC.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreMVVM.IOC.Core
{
    internal class LifetimeScope : ILifetimeScope
    {
        private readonly IReadOnlyDictionary<Type, IRegistration> _registeredTypes;
        private readonly ICollection<IDisposable> _disposables = new List<IDisposable>();

        private readonly object _disposeLock = new object();

        public LifetimeScope(IReadOnlyDictionary<Type, IRegistration> registeredTypes)
        {
            _registeredTypes = registeredTypes;
        }

        #region Properties

        public bool IsDisposed { get; private set; }

        #endregion Properties

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
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ILifetimeScope));

            bool isRegistered = _registeredTypes.TryGetValue(type, out IRegistration registration);
            if (isRegistered)
            {
                if (registration.IsSingleton)
                {
                    lock (registration)
                    {
                        if (registration.SingletonInstance == null)
                            registration.SingletonInstance = ConstructFromRegistration(registration);

                        return registration.SingletonInstance;
                    }
                }

                return ConstructFromRegistration(registration);
            }

            return ConstructType(type);
        }

        /// <summary>
        /// Creates a new lifetime scope.
        /// </summary>
        /// <returns>A new lifetime scope.</returns>
        public ILifetimeScope BeginLifetimeScope()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ILifetimeScope));

            ILifetimeScope childScope = new LifetimeScope(_registeredTypes);
            _disposables.Add(childScope);

            return childScope;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            lock (_disposeLock)
            {
                if (IsDisposed)
                    return;

                IsDisposed = true;
                foreach (IDisposable disposable in _disposables)
                    disposable.Dispose();

                _disposables.Clear();
            }
        }

        #endregion Methods

        #region Construct methods

        private object ConstructFromRegistration(IRegistration registration)
        {
            if (registration.Factory != null)
                return registration.Factory(this);

            return ConstructType(registration.Type);
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

                object instance = constructor.Invoke(args);
                if (instance is IDisposable disposable)
                    _disposables.Add(disposable);

                return instance;
            }
            catch (Exception e)
            {
                string message = $"Failed to construct instance of type '{type}'.";

                Resolve<ILogger>().Exception(message, e);
                throw new ResolveConstructionException(message, e);
            }
        }

        #endregion Construct methods
    }
}