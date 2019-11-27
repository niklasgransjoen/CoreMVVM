using CoreMVVM.Extentions;
using CoreMVVM.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CoreMVVM.IOC.Core
{
    internal class LifetimeScope : ILifetimeScope
    {
        private readonly ToolBox _toolBox;
        private readonly ICollection<IDisposable> _disposables = new List<IDisposable>();
        private readonly LifetimeScope _parent;
        private readonly IResolveUnregisteredInterfaceService _resolveUnregisteredInterfaceService;

        private readonly object _disposeLock = new object();

        public LifetimeScope(ToolBox toolBox)
        {
            _toolBox = toolBox;
            _resolveUnregisteredInterfaceService = Resolve<IResolveUnregisteredInterfaceService>();
        }

        public LifetimeScope(ToolBox toolBox, LifetimeScope parent)
            : this(toolBox)
        {
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

            ILifetimeScope childScope = new LifetimeScope(_toolBox, this);
            _disposables.Add(childScope);

            return childScope;
        }

        #endregion Methods

        #region IDispose

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

        #endregion IDispose

        #region Private resolve

        private object Resolve(Type type, bool isOwned)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ILifetimeScope));

            if (TryResolveRegisteredComponent(type, isOwned, out object component))
                return component;
            else if (TryResolveComponentByAttribute(type, isOwned, out component))
                return component;
            else
            {
                object instance = ConstructType(type, isOwned);
                if (instance != null)
                    InitializeComponent(instance);

                return instance;
            }
        }

        /// <summary>
        /// Attempts to resolve the component from the toolbox's registrations.
        /// </summary>
        private bool TryResolveRegisteredComponent(Type type, bool isOwned, out object component)
        {
            if (!_toolBox.TryGetRegistration(type, out IRegistration registration))
            {
                component = null;
                return false;
            }

            component = ResolveFromRegistration(type, isOwned, registration);
            return true;
        }

        /// <summary>
        /// Attempts to resolve the component by checking for a <see cref="ScopeAttribute"/>.
        /// </summary>
        private bool TryResolveComponentByAttribute(Type type, bool isOwned, out object component)
        {
            var attribute = type.GetCustomAttribute<ScopeAttribute>();
            if (attribute is null)
            {
                component = null;
                return false;
            }

            Registration registration = new Registration(type)
            {
                Scope = attribute.Scope,
            };
            _toolBox.AddRegistration(type, registration);
            component = ResolveFromRegistration(type, isOwned, registration);
            return true;
        }

        private object ResolveFromRegistration(Type type, bool isOwned, IRegistration registration)
        {
            if (registration.Scope == ComponentScope.None)
            {
                object component = ConstructFromRegistration(registration, isOwned);
                if (component != null)
                    InitializeComponent(component);

                return component;
            }

            if (isOwned)
                throw new OwnedScopedComponentException($"Attempted to own component of type '{type}', with scope '{registration.Scope}'. Scoped components cannot be owned.");

            return ResolveScopedComponent(registration);
        }

        private object ResolveScopedComponent(IRegistration registration)
        {
            // Singletons should only be resolved by root.
            if (registration.Scope == ComponentScope.Singleton && _parent != null)
                return _parent.ResolveScopedComponent(registration);

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
            // Resolve ILifetimeScope
            if (TryConstructILifetimeScope(type, out ILifetimeScope lifetimeScope))
                return lifetimeScope;

            // Resolve Func<T> to factory.
            if (TryConstructFactory(type, isOwned, out Func<object> factory))
                return factory;

            // Resolve Lazy<T>
            if (TryConstructLazy(type, isOwned, out object lazyInstance))
                return lazyInstance;

            // Switch out any IOwned<> (or implementation) with Owned<>
            bool implementsIOwned = type.ImplementsGenericInterface(typeof(IOwned<>));
            if (implementsIOwned)
                type = typeof(Owned<>).MakeGenericType(type.GenericTypeArguments);

            // Check if type is unregistered interface.
            if (type.IsInterface)
            {
                var context = new ResolveUnregisteredInterfaceContext(type);
                _resolveUnregisteredInterfaceService.Handle(context);

                if (context.InterfaceImplementationType is null)
                    throw new ResolveUnregisteredInterfaceException($"Failed to resolve unregistered interface '{type}'.");

                if (context.CacheImplementation)
                    _toolBox.AddRegistration(type, new Registration(context.InterfaceImplementationType));

                return ConstructType(context.InterfaceImplementationType, isOwned);
            }

            // Construct type.
            try
            {
                if (!_toolBox.TryGetConstructor(type, out ConstructorInfo constructor))
                {
                    ConstructorInfo[] constructors = type.GetConstructors();
                    if (constructors.Length == 0)
                        return type.GetDefault();

                    constructor = constructors
                        .OrderByDescending(c => c.GetParameters().Length)
                        .First();

                    _toolBox.AddConstructor(type, constructor);
                    _toolBox.AddParameterInfo(constructor);
                }

                _toolBox.TryGetParameterInfo(constructor, out ParameterInfo[] parameters);
                Debug.Assert(parameters != null);

                object[] args = parameters.Select(param => Resolve(param.ParameterType, implementsIOwned))
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

        private bool TryConstructILifetimeScope(Type type, out ILifetimeScope lifetimeScope)
        {
            if (type != typeof(ILifetimeScope))
            {
                lifetimeScope = null;
                return false;
            }

            lifetimeScope = this;
            return true;
        }

        private bool TryConstructFactory(Type factoryType, bool isOwned, out Func<object> factory)
        {
            bool isFactory = factoryType.IsGenericType && factoryType.GetGenericTypeDefinition() == typeof(Func<>);
            if (!isFactory)
            {
                factory = null;
                return false;
            }

            factory = ConstructFactory(factoryType, isOwned);
            return true;
        }

        private Func<object> ConstructFactory(Type factoryType, bool isOwned)
        {
            Type resultType = factoryType.GenericTypeArguments[0];
            Expression<Func<object>> factoryExpression = () => Resolve(resultType, isOwned);
            Expression factoryBody = Expression.Invoke(factoryExpression);
            Expression convertedResult = Expression.Convert(factoryBody, resultType);

            return (Func<object>)Expression.Lambda(factoryType, convertedResult).Compile();
        }

        private bool TryConstructLazy(Type type, bool isOwned, out object lazyInstance)
        {
            bool isLazy = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>);
            if (!isLazy)
            {
                lazyInstance = null;
                return false;
            }

            lazyInstance = ConstructLazy(type, isOwned);
            return true;
        }

        private object ConstructLazy(Type type, bool isOwned)
        {
            ConstructorInfo[] constructors = type.GetConstructors();

            ConstructorInfo constructor = null;
            ParameterInfo parameter = null;
            foreach (ConstructorInfo c in constructors)
            {
                ParameterInfo[] parameters = c.GetParameters();
                if (parameters.Length == 1)
                {
                    Type paramType = parameters[0].ParameterType;
                    if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(Func<>) &&
                        paramType.GenericTypeArguments[0] == type.GenericTypeArguments[0])
                    {
                        constructor = c;
                        parameter = parameters[0];

                        break;
                    }
                }
            }

            Func<object> factory = ConstructFactory(parameter.ParameterType, isOwned);
            object[] args = new object[] { factory };

            return constructor.Invoke(args);
        }

        private void RegisterDisposable(object instance)
        {
            if (instance is IDisposable disposable)
                _disposables.Add(disposable);
        }

        /// <summary>
        /// Invokes the "InitializedComponent" method on the given component, if such a method exists.
        /// </summary>
        /// <param name="element">The component to initialize. Not null.</param>
        private void InitializeComponent(object element)
        {
            if (element is IComponent component)
                component.InitializeComponent(this);
        }

        #endregion Construct methods
    }
}