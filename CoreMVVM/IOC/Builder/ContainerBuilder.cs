using CoreMVVM.IOC.Core;
using System;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// For building a container.
    /// </summary>
    public sealed class ContainerBuilder
    {
        private readonly ToolBox _toolBox = new ToolBox();

        #region Constructors

        /// <summary>
        /// Creates a new container builder.
        /// </summary>
        public ContainerBuilder()
        {
            Register<FallbackImplementations.UnregisteredInterfaceFallbackService>(ComponentScope.Singleton)
                .As<IResolveUnregisteredInterfaceService>();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when the container is built.
        /// </summary>
        public event Action<IContainer>? OnBuild;

        #endregion Events

        #region Methods

        /// <summary>
        /// Registers a component.
        /// </summary>
        /// <typeparam name="T">The type of component to register.</typeparam>
        /// <param name="scope">The scope to register the component in.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public IRegistrationBuilder<T> Register<T>(ComponentScope scope)
            where T : class
        {
            return new RegistrationBuilder<T>(_toolBox, scope);
        }

        /// <summary>
        /// Registers a component.
        /// </summary>
        /// <typeparam name="T">The type of component to register.</typeparam>
        /// <param name="scope">The scope to register the component in.</param>
        /// <param name="factory">The factory to create the service with.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public IRegistrationBuilder<T> Register<T>(ComponentScope scope, Func<ILifetimeScope, T> factory)
            where T : class
        {
            return new RegistrationBuilder<T>(_toolBox, scope, factory);
        }

        /// <summary>
        /// Registers a component.
        /// </summary>
        /// <param name="type">The type of the component to register.</param>
        /// <param name="scope">The scope to register the component in.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public IRegistrationBuilder Register(Type type, ComponentScope scope)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return new RegistrationBuilder(_toolBox, type, scope);
        }

        /// <summary>
        /// Registers a component.
        /// </summary>
        /// <param name="type">The type of the component to register.</param>
        /// <param name="scope">The scope to register the component in.</param>
        /// <param name="factory">The factory to create the service with.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public IRegistrationBuilder Register(Type type, ComponentScope scope, Func<ILifetimeScope, object> factory)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return new RegistrationBuilder(_toolBox, type, scope, factory);
        }

        /// <summary>
        /// Constructs a container with all the registered components and services.
        /// </summary>
        public IContainer Build()
        {
            Container container = new Container(_toolBox);

            OnBuild?.Invoke(container);

            return container;
        }

        #endregion Methods
    }
}