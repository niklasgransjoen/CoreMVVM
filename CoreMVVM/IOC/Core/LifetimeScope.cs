using CoreMVVM.Extentions;
using CoreMVVM.IOC.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CoreMVVM.IOC.Core
{
    internal class LifetimeScope : ILifetimeScope
    {
        private readonly IReadOnlyDictionary<Type, IRegistration> _registeredTypes;
        private readonly ICollection<IDisposable> _disposables = new List<IDisposable>();
        private readonly LifetimeScope _parent;

        private readonly object _disposeLock = new object();

        public LifetimeScope(IReadOnlyDictionary<Type, IRegistration> registeredTypes)
        {
            _registeredTypes = registeredTypes;
        }

        public LifetimeScope(IReadOnlyDictionary<Type, IRegistration> registeredTypes, LifetimeScope parent)
        {
            _registeredTypes = registeredTypes;
            _parent = parent;
        }

        #region Properties

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets a collection of resolved instances with limited scoping.
        /// </summary>
        public Dictionary<IRegistration, object> ResolvedInstances { get; } = new Dictionary<IRegistration, object>();

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <typeparam name="T">The type to get an instance for.</typeparam>
        /// <exception cref="ResolveUnregisteredInterfaceException">T is an unregistered or resolves to an interface.</exception>
        /// <exception cref="ResolveConstructionException">Fails to construct type or one of its arguments.</exception>
        public T Resolve<T>() => (T)Resolve(typeof(T), isOwned: false);

        /// <summary>
        /// Returns an instance from the given type.
        /// </summary>
        /// <param name="type">The type to get an instance for.</param>
        /// <exception cref="ResolveUnregisteredInterfaceException">type is an unregistered or resolves to an interface.</exception>
        /// <exception cref="ResolveConstructionException">Fails to construct type or one of its arguments.</exception>
        public object Resolve(Type type) => Resolve(type, isOwned: false);

        /// <summary>
        /// Creates a new lifetime scope.
        /// </summary>
        /// <returns>A new lifetime scope.</returns>
        public ILifetimeScope BeginLifetimeScope()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ILifetimeScope));

            ILifetimeScope childScope = new LifetimeScope(_registeredTypes, this);
            _disposables.Add(childScope);

            return childScope;
        }

        /// <summary>
        /// Disposes this LifetimeScope, along with all of its subscopes,
        /// as well as all instances resolved by it.
        /// </summary>
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

        #region Private resolve

        private object Resolve(Type type, bool isOwned)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ILifetimeScope));

            bool isRegistered = _registeredTypes.TryGetValue(type, out IRegistration registration);
            if (isRegistered)
            {
                if (registration.Scope == InstanceScope.None)
                {
                    object instance = ConstructFromRegistration(registration, isOwned);
                    if (instance != null)
                        InitializeComponent(instance);

                    return instance;
                }

                if (isOwned)
                    throw new OwnedScopedComponentException($"Attempted to own component of type '{type}', with scope '{registration.Scope}'. Scoped components cannot be owned.");

                return ResolveScopedComponent(registration, type);
            }
            else
            {
                object instance = ConstructType(type, isOwned);
                if (instance != null)
                    InitializeComponent(instance);

                return instance;
            }
        }

        private object ResolveScopedComponent(IRegistration registration, Type type)
        {
            // Singletons should only be resolved by root.
            if (registration.Scope == InstanceScope.Singleton && _parent != null)
                return _parent.ResolveScopedComponent(registration, type);

            // Result from scoped components are saved for future resolves.
            lock (registration)
            {
                if (!ResolvedInstances.TryGetValue(registration, out object instance))
                {
                    instance = ConstructFromRegistration(registration, isOwned: false);
                    ResolvedInstances[registration] = instance;

                    if (instance != null)
                        InitializeComponent(instance);
                }

                return instance;
            }
        }

        #endregion Private resolve

        #region Construct methods

        private object ConstructFromRegistration(IRegistration registration, bool isOwned)
        {
            if (registration.Factory != null)
            {
                object instance = registration.Factory(this);

                if (!isOwned)
                    RegisterDisposable(instance);

                return instance;
            }

            return ConstructType(registration.Type, isOwned);
        }

        /// <summary>
        /// Constructs an instance of the given type, using the constructor with the most parameters.
        /// </summary>
        /// <param name="type">The type of component to construct.</param>
        /// <param name="isOwned">Indicates if the caller to resolve should own the constructed component,
        /// instead of this lifetimescope.</param>
        /// <exception cref="ResolveUnregisteredInterfaceException">type is an interface.</exception>
        /// <exception cref="ResolveConstructionException">Fails to construct type.</exception>
        private object ConstructType(Type type, bool isOwned)
        {
            // Transform Func<T> into a factory.
            if (TryConstructFactory(type, isOwned, out Func<object> factory))
                return factory;

            // Switch out any IOwned<> (or implementation) with Owned<>
            bool implementsIOwned = type.ImplementsGenericInterface(typeof(IOwned<>));
            if (implementsIOwned)
                type = typeof(Owned<>).MakeGenericType(type.GenericTypeArguments);

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
                                           .Select(param => Resolve(param.ParameterType, implementsIOwned))
                                           .ToArray();

                object instance = constructor.Invoke(args);

                if (!isOwned)
                    RegisterDisposable(instance);

                return instance;
            }
            catch (OwnedScopedComponentException)
            {
                throw;
            }
            catch (Exception e)
            {
                string message = $"Failed to construct instance of type '{type}'.";

                Resolve<ILogger>().Exception(message, e);
                throw new ResolveConstructionException(message, e);
            }
        }

        private bool TryConstructFactory(Type type, bool isOwned, out Func<object> factory)
        {
            bool isFactory = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>);
            if (!isFactory)
            {
                factory = null;
                return false;
            }

            Type resultType = type.GenericTypeArguments[0];
            Expression<Func<object>> factoryExpression = () => Resolve(resultType, isOwned);
            Expression factoryBody = Expression.Invoke(factoryExpression);
            Expression convertedResult = Expression.Convert(factoryBody, resultType);

            factory = (Func<object>)Expression.Lambda(type, convertedResult).Compile();
            return true;
        }

        private void RegisterDisposable(object instance)
        {
            if (instance is IDisposable disposable)
                _disposables.Add(disposable);
        }

        /// <summary>
        /// Invokes the "InitializedComponent" method on the given component, if such a method exists.
        /// </summary>
        /// <param name="component">The component to initialize. Not null.</param>
        private void InitializeComponent(object element)
        {
            if (element is IComponent component)
                component.InitializeComponent();
        }

        #endregion Construct methods
    }
}