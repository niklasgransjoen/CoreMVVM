using CoreMVVM.Implementations;
using CoreMVVM.IOC.Core;
using CoreMVVM.Services;
using System;
using System.Linq;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// For building a container.
    /// </summary>
    public class ContainerBuilder
    {
        private readonly RegistrationCollection _registrations = new RegistrationCollection();

        #region Constructors

        /// <summary>
        /// Creates a new container builder.
        /// </summary>
        public ContainerBuilder()
        {
            RegisterSingleton<UnregisteredInterfaceFallbackService>().As<IResolveUnregisteredInterfaceService>();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when the container is built.
        /// </summary>
        public event Action<IContainer> OnBuild;

        #endregion Events

        #region Methods

        #region No scope

        /// <summary>
        /// Registers a component.
        /// </summary>
        /// <typeparam name="T">The type of the component to register.</typeparam>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        /// <exception cref="ScopingConflictException">T is already registered with a different scope.</exception>
        public IRegistrationBuilder Register<T>() => Register(typeof(T));

        /// <summary>
        /// Registers a component.
        /// </summary>
        /// <param name="type">The type of the component to register.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        /// <exception cref="ScopingConflictException">type is already registered with a different scope.</exception>
        public IRegistrationBuilder Register(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            ThrowOnScopingConflict(type, InstanceScope.None);

            return RegistrationBuilder.Create(_registrations, type);
        }

        /// <summary>
        /// Registers a component.
        /// </summary>
        /// <typeparam name="T">The type of component to register.</typeparam>
        /// <param name="factory">A factory to use for constructing this component.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        /// <exception cref="ScopingConflictException">T is already registered with a different scope.</exception>
        public IRegistrationBuilder Register<T>(Func<ILifetimeScope, T> factory) where T : class
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            ThrowOnScopingConflict<T>(InstanceScope.None);

            return RegistrationBuilder.Create(_registrations, factory);
        }

        #endregion No scope

        #region Singleton

        /// <summary>
        /// Registers a component as a singleton.
        /// </summary>
        /// <typeparam name="T">The type of the component to register.</typeparam>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        /// <exception cref="ScopingConflictException">T is already registered with a different scope.</exception>
        public IRegistrationBuilder RegisterSingleton<T>() => RegisterSingleton(typeof(T));

        /// <summary>
        /// Registers a component as a singleton.
        /// </summary>
        /// <param name="type">The type of the component to register.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        /// <exception cref="ScopingConflictException">type is already registered with a different scope.</exception>
        public IRegistrationBuilder RegisterSingleton(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            ThrowOnScopingConflict(type, InstanceScope.Singleton);

            return RegistrationBuilder.CreateSingleton(_registrations, type);
        }

        /// <summary>
        /// Registers a component as a singleton.
        /// </summary>
        /// <typeparam name="T">The type of component to register.</typeparam>
        /// <param name="factory">A factory to use for constructing this component.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        /// <exception cref="ScopingConflictException">T is already registered with a different scope.</exception>
        public IRegistrationBuilder RegisterSingleton<T>(Func<ILifetimeScope, T> factory) where T : class
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            ThrowOnScopingConflict<T>(InstanceScope.Singleton);

            return RegistrationBuilder.CreateSingleton(_registrations, factory);
        }

        #endregion Singleton

        #region Lifetime scope

        /// <summary>
        /// Registers a component with a lifetime scope.
        /// </summary>
        /// <typeparam name="T">The type of the component to register.</typeparam>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        /// <exception cref="ScopingConflictException">T is already registered with a different scope.</exception>
        public IRegistrationBuilder RegisterLifetimeScope<T>() => RegisterLifetimeScope(typeof(T));

        /// <summary>
        /// Registers a component with a lifetime scope.
        /// </summary>
        /// <param name="type">The type of the component to register.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        /// <exception cref="ScopingConflictException">type is already registered with a different scope.</exception>
        public IRegistrationBuilder RegisterLifetimeScope(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            ThrowOnScopingConflict(type, InstanceScope.LifetimeScope);

            return RegistrationBuilder.CreateLifetimeScope(_registrations, type);
        }

        /// <summary>
        /// Registers a component with a lifetime scope.
        /// </summary>
        /// <typeparam name="T">The type of component to register.</typeparam>
        /// <param name="factory">A factory to use for constructing this component.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        /// <exception cref="ScopingConflictException">T is already registered with a different scope.</exception>
        public IRegistrationBuilder RegisterLifetimeScope<T>(Func<ILifetimeScope, T> factory) where T : class
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            ThrowOnScopingConflict<T>(InstanceScope.LifetimeScope);

            return RegistrationBuilder.CreateLifetimeScope(_registrations, factory);
        }

        #endregion Lifetime scope

        /// <summary>
        /// Constructs a container with all the registered components and services.
        /// </summary>
        public IContainer Build()
        {
            // Registers the container as a singleton, so it always resolves to this instance.
            RegisterSingleton<Container>().As<IContainer>().AsSelf();

            ToolBox toolBox = new ToolBox(_registrations);
            Container container = new Container(toolBox);

            // And set this instance as the last created one, so this is the one that's returned upon a call to IContainer.Resolve().
            IRegistration registration = _registrations[typeof(IContainer)];
            container.ResolvedInstances[registration] = container;

            OnBuild?.Invoke(container);

            return container;
        }

        #endregion Methods

        #region Helper

        private void ThrowOnScopingConflict<T>(InstanceScope scope) => ThrowOnScopingConflict(typeof(T), scope);

        private void ThrowOnScopingConflict(Type type, InstanceScope scope)
        {
            // Verify that all registrations have the same scope.
            var previousRegs = _registrations.Where(r => r.Value.Type == type);
            if (previousRegs.All(r => r.Value.Scope == scope))
                return;

            var previousReg = previousRegs.First();
            string message =
                $"Attempted to register type '{type}' with scope '{scope}', " +
                $"which conflicts with earlier registration as a component of '{previousReg.Key}', with with scope '{previousReg.Value.Scope}'.";

            throw new ScopingConflictException(message);
        }

        #endregion Helper
    }
}