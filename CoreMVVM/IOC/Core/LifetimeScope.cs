using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CoreMVVM.IOC.Core
{
    internal class LifetimeScope : ILifetimeScope
    {
        private readonly ToolBox _toolBox;
        private readonly LifetimeScope? _parent;
        private readonly IResolveUnregisteredInterfaceService? _resolveUnregisteredInterfaceService;

        private readonly ICollection<IDisposable> _disposables = new List<IDisposable>();
        private readonly HashSet<IRegistration> _resolvingScopedComponents = new HashSet<IRegistration>();

        public LifetimeScope(ToolBox toolBox)
        {
            _toolBox = toolBox;
            _resolveUnregisteredInterfaceService = this.ResolveService<IResolveUnregisteredInterfaceService>();
        }

        public LifetimeScope(ToolBox toolBox, LifetimeScope parent)
            : this(toolBox)
        {
            _parent = parent;
        }

        #region Properties

        /// <summary>
        /// Gets a collection of resolved instances with limited scoping.
        /// </summary>
        public Dictionary<(Type concreteType, IRegistration registration), object> ResolvedInstances { get; } = new Dictionary<(Type, IRegistration), object>();

        #endregion Properties

        #region ILifetimeScope

        /// <summary>
        /// Gets a value indicating if this container has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

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

        public object? GetService(Type serviceType)
        {
            return Resolve(serviceType, isOwned: false);
        }

        #endregion ILifetimeScope

        #region IDispose

        /// <summary>
        /// Disposes this LifetimeScope, along with all of its subscopes,
        /// as well as all instances resolved by it.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            foreach (IDisposable disposable in _disposables)
                disposable.Dispose();

            _disposables.Clear();
        }

        #endregion IDispose

        #region Private resolve

        private object? Resolve(Type type, bool isOwned)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ILifetimeScope));

            if (type.IsValueType)
                throw new ArgumentException($"Cannot resolve value type (Type '{type}').");

            if (TryResolveRegisteredComponent(type, isOwned, out var component))
                return component;
            else if (TryResolveComponentByAttribute(type, isOwned, out component))
                return component;
            else if (TryResolveRegistration(type, out var registration))
            {
                component = ResolveFromRegistration(type, registration, isOwned);
                InitializeComponent(component);

                return component;
            }

            return null;
        }

        /// <summary>
        /// Attempts to resolve the component from the toolbox's registrations.
        /// </summary>
        private bool TryResolveRegisteredComponent(Type serviceType, bool isOwned, [NotNullWhen(true)] out object? component)
        {
            if (!_toolBox.TryGetRegistration(serviceType, out var registration))
            {
                component = null;
                return false;
            }

            component = ResolveFromRegistration(serviceType, registration, isOwned);
            return component != null;
        }

        /// <summary>
        /// Attempts to resolve the component by checking for a <see cref="ScopeAttribute"/>.
        /// </summary>
        private bool TryResolveComponentByAttribute(Type serviceType, bool isOwned, [NotNullWhen(true)] out object? component)
        {
            var attribute = serviceType.GetCustomAttribute<ScopeAttribute>();
            if (attribute is null)
            {
                component = null;
                return false;
            }

            // Register type as itself.
            var registration = _toolBox.AddRegistration(serviceType, serviceType, attribute.Scope);
            component = ResolveFromRegistration(serviceType, registration, isOwned);
            return component != null;
        }

        internal static readonly IRegistration IContainerRegistration = new Registration(typeof(Container), ComponentScope.Singleton);
        internal static readonly IRegistration IServiceProviderRegistration = new Registration(typeof(LifetimeScope), ComponentScope.LifetimeScope);
        internal static readonly IRegistration IEnumerableRegistration = new Registration(typeof(List<>), ComponentScope.Transient);
        internal static readonly IRegistration FuncRegistration = new Registration(typeof(Func<>), ComponentScope.Transient);
        internal static readonly IRegistration LazyRegistration = new Registration(typeof(Lazy<>), ComponentScope.Transient);
        internal static readonly IRegistration IOwnedRegistration = new Registration(typeof(Owned<>), ComponentScope.Transient);

        private bool TryResolveRegistration(Type type, [NotNullWhen(true)] out IRegistration? registration)
        {
            if (_toolBox.TryGetRegistration(type, out registration))
                return true;

            if (type == typeof(IContainer))
            {
                registration = IContainerRegistration;
                return true;
            }
            else if (type == typeof(IServiceProvider) || type == typeof(ILifetimeScope))
            {
                registration = IServiceProviderRegistration;
                return true;
            }

            if (type.IsGenericType)
            {
                Type genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(IEnumerable<>))
                {
                    registration = IEnumerableRegistration;
                    return true;
                }

                if (genericType == typeof(Func<>))
                {
                    registration = FuncRegistration;

                    var genericArgument = type.GenericTypeArguments[0];
                    return TryResolveRegistration(genericArgument, out _);
                }
                else if (genericType == typeof(Lazy<>))
                {
                    registration = LazyRegistration;

                    var genericArgument = type.GenericTypeArguments[0];
                    return TryResolveRegistration(genericArgument, out _);
                }
                else if (genericType == typeof(IOwned<>))
                {
                    registration = IOwnedRegistration;

                    var genericArgument = type.GenericTypeArguments[0];
                    return TryResolveRegistration(genericArgument, out _);
                }
            }

            // Check if type is unregistered interface.
            if (type.IsInterface)
            {
                if (_resolveUnregisteredInterfaceService is null)
                {
                    registration = null;
                    return false;
                }

                var context = new ResolveUnregisteredInterfaceContext(type);
                _resolveUnregisteredInterfaceService.Handle(context);

                if (context.InterfaceImplementationType is null)
                {
                    registration = null;
                    return false;
                }

                if (context.CacheImplementation)
                {
                    registration = _toolBox.AddRegistration(context.InterfaceImplementationType, type, context.CacheScope);
                    return true;
                }

                registration = new Registration(context.InterfaceImplementationType, ComponentScope.Transient);
                return true;
            }
            else if (type.IsClass && !type.IsAbstract)
            {
                registration = _toolBox.AddRegistration(type, type, ComponentScope.Transient);
                return true;
            }

            registration = null;
            return false;
        }

        private object? ResolveFromRegistration(Type serviceType, IRegistration registration, bool isOwned)
        {
            if (registration.Scope == ComponentScope.Transient)
            {
                var component = ConstructFromRegistration(serviceType, registration, isOwned);
                if (component != null)
                    InitializeComponent(component);

                return component;
            }

            if (isOwned)
                throw new OwnedScopedComponentException($"Attempted to own component of type '{registration.Type}', with scope '{registration.Scope}'. Scoped components cannot be owned.");

            return ResolveScopedComponent(serviceType, registration);
        }

        private object? ResolveScopedComponent(Type serviceType, IRegistration registration)
        {
            // Singletons should only be resolved by root.
            if (registration.Scope == ComponentScope.Singleton && _parent != null)
                return _parent.ResolveScopedComponent(serviceType, registration);

            lock (registration)
            {
                // Result from scoped components are saved for future resolves.
                var concreteType = registration.GetConcreteType(serviceType);
                if (!ResolvedInstances.TryGetValue((concreteType, registration), out var instance))
                {
                    if (!_resolvingScopedComponents.Add(registration))
                    {
                        throw new ResolveException(
                            $"Recursive pattern on scoped components detected. " +
                            $"This was detected while resolving service '{registration.Type}'.");
                    }

                    try
                    {
                        instance = ConstructFromRegistration(serviceType, registration, isOwned: false);
                        if (ResolvedInstances.ContainsKey((concreteType, registration)))
                        {
                            throw new ResolveException(
                                $"Recursive pattern on scoped components detected. " +
                                $"This was detected while resolving service '{registration.Type}'.");
                        }

                        if (instance != null)
                        {
                            ResolvedInstances.Add((concreteType, registration), instance);
                        }
                        InitializeComponent(instance);
                    }
                    finally
                    {
                        _resolvingScopedComponents.Remove(registration);
                    }
                }

                return instance;
            }
        }

        #endregion Private resolve

        #region Construct methods

        /// <summary>
        /// Constructs an instance of the given type.
        /// </summary>
        /// <param name="serviceType">The service to resolve.</param>
        /// <param name="registration">Registration info.</param>
        /// <param name="isOwned">Indicates if the caller to resolve should own the constructed component,
        /// instead of this lifetimescope.</param>
        /// <exception cref="IOCException">Construction fails.</exception>
        private object? ConstructFromRegistration(Type serviceType, IRegistration registration, bool isOwned)
        {
            if (registration.Factory != null)
            {
                object instance = registration.Factory(this);
                if (instance is null)
                    throw new ResolveException($"Factory for service '{registration.Type}' returned null.");

                if (!isOwned)
                    RegisterDisposable(instance);

                return instance;
            }

            var concreteType = registration.GetConcreteType(serviceType);

            // Resolve IServiceProvider or similar.
            if (TryConstructServiceProvider(serviceType, out var serviceProvider))
                return serviceProvider;

            // Resolve IEnumerable<T> to a sequence of services
            if (TryConstructIEnumerable(serviceType, out var serviceSequence))
                return serviceSequence;

            // Resolve Func<T> to factory.
            if (TryConstructFactory(serviceType, isOwned, out var factory))
                return factory;

            // Resolve Lazy<T>
            if (TryConstructLazy(serviceType, isOwned, out var lazyInstance))
                return lazyInstance;

            // Handle IOwned<>
            bool implementsIOwned = false;
            if (concreteType.IsGenericType)
            {
                Type genericType = concreteType.GetGenericTypeDefinition();
                implementsIOwned = typeof(IOwned<>).IsAssignableFromGeneric(genericType);
            }

            // Construct type.
            try
            {
                var constructor = _toolBox.GetConstructor(concreteType);
                var parameters = _toolBox.GetParameterInfo(constructor);

                object[] args = parameters.Select(param => Resolve(param.ParameterType, implementsIOwned) ?? throw new ResolveUnregisteredServiceException($"No service for type '{param.ParameterType}' has been registered."))
                                          .ToArray();

                object instance = constructor.Invoke(args);

                if (!isOwned)
                    RegisterDisposable(instance);

                return instance;
            }
            catch (IOCException)
            {
                throw;
            }
            catch (Exception e)
            {
                string message = $"Failed to construct instance of type '{concreteType}'.";

                throw new ResolveException(message, e);
            }
        }

        private bool TryConstructIEnumerable(Type type, [NotNullWhen(true)] out IEnumerable? services)
        {
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(IEnumerable<>))
            {
                services = null;
                return false;
            }

            var serviceType = type.GenericTypeArguments[0];
            Type listType = typeof(List<>).MakeGenericType(serviceType);

            var listConstructor = _toolBox.GetConstructor(listType, validateParameters: false, constructorSelector: ctors => ctors.Single(ctor =>
            {
                var parameters = ctor.GetParameters();
                if (parameters.Length != 1)
                    return false;

                return parameters[0].ParameterType == typeof(int);
            }));

            var registrations = _toolBox.GetRegistrations(serviceType);

            var serviceCount = registrations.Count;
            var list = (IList)listConstructor.Invoke(new object[] { serviceCount });

            foreach (var registration in registrations)
            {
                list.Add(ResolveFromRegistration(serviceType, registration, isOwned: false));
            }

            services = list;
            return true;
        }

        private bool TryConstructServiceProvider(Type type, [NotNullWhen(true)] out IServiceProvider? serviceProvider)
        {
            if (type == typeof(IContainer))
            {
                if (_parent is null)
                {
                    serviceProvider = this;
                }
                else
                {
                    _parent.TryConstructServiceProvider(type, out serviceProvider!);
                }

                return true;
            }
            if (type == typeof(ILifetimeScope) || type == typeof(IServiceProvider))
            {
                serviceProvider = this;
                return true;
            }

            serviceProvider = null;
            return false;
        }

        private bool TryConstructFactory(Type factoryType, bool isOwned, [NotNullWhen(true)] out Func<object>? factory)
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
            if (resultType.IsValueType)
                throw new ResolveException($"Cannot resolve factory for value type '{resultType}'.");

            Expression<Func<object>> factoryExpression = () => Resolve(resultType, isOwned)!; // TODO: #4
            Expression factoryBody = Expression.Invoke(factoryExpression);
            Expression convertedResult = Expression.Convert(factoryBody, resultType);

            return (Func<object>)Expression.Lambda(factoryType, convertedResult).Compile();
        }

        private bool TryConstructLazy(Type type, bool isOwned, [NotNullWhen(true)] out object? lazyInstance)
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

            ConstructorInfo constructor = null!;
            ParameterInfo parameter = null!;
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
        private void InitializeComponent(object? element)
        {
            if (element is IComponent component)
                component.InitializeComponent(this);
        }

        #endregion Construct methods
    }
}