using System;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// Extension methods for <see cref="ContainerBuilder"/>.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        #region Transient

        /// <summary>
        /// Registers a component.
        /// </summary>
        /// <typeparam name="T">The type of the component to register.</typeparam>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public static IRegistrationBuilder RegisterTransient<T>(this ContainerBuilder builder) => RegisterTransient(builder, typeof(T));

        /// <summary>
        /// Registers a component.
        /// </summary>
        /// <param name="type">The type of the component to register.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public static IRegistrationBuilder RegisterTransient(this ContainerBuilder builder, Type type)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));

            return builder.Register(type, ComponentScope.Transient);
        }

        /// <summary>
        /// Registers a component.
        /// </summary>
        /// <typeparam name="T">The type of component to register.</typeparam>
        /// <param name="factory">A factory to use for constructing this component.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public static IRegistrationBuilder RegisterTransient<T>(this ContainerBuilder builder, Func<ILifetimeScope, T> factory) where T : class
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return builder.Register(ComponentScope.Transient, factory);
        }

        /// <summary>
        /// Registers a component.
        /// </summary>
        /// <param name="type">The type of the component to register.</param>
        /// <param name="factory">A factory to use for constructing this component.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public static IRegistrationBuilder RegisterTransient(this ContainerBuilder builder, Type type, Func<ILifetimeScope, object> factory)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return builder.Register(type, ComponentScope.Transient, factory);
        }

        #endregion Transient

        #region Lifetime scope

        /// <summary>
        /// Registers a component with a lifetime scope.
        /// </summary>
        /// <typeparam name="T">The type of the component to register.</typeparam>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public static IRegistrationBuilder RegisterLifetimeScope<T>(this ContainerBuilder builder) => RegisterLifetimeScope(builder, typeof(T));

        /// <summary>
        /// Registers a component with a lifetime scope.
        /// </summary>
        /// <param name="type">The type of the component to register.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public static IRegistrationBuilder RegisterLifetimeScope(this ContainerBuilder builder, Type type)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));

            return builder.Register(type, ComponentScope.LifetimeScope);
        }

        /// <summary>
        /// Registers a component with a lifetime scope.
        /// </summary>
        /// <typeparam name="T">The type of component to register.</typeparam>
        /// <param name="factory">A factory to use for constructing this component.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public static IRegistrationBuilder RegisterLifetimeScope<T>(this ContainerBuilder builder, Func<ILifetimeScope, T> factory) where T : class
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return builder.Register(ComponentScope.LifetimeScope, factory);
        }

        /// <summary>
        /// Registers a component with a lifetime scope.
        /// </summary>
        /// <param name="type">The type of the component to register.</param>
        /// <param name="factory">A factory to use for constructing this component.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public static IRegistrationBuilder RegisterLifetimeScope(this ContainerBuilder builder, Type type, Func<ILifetimeScope, object> factory)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return builder.Register(type, ComponentScope.LifetimeScope, factory);
        }

        #endregion Lifetime scope

        #region Singleton

        /// <summary>
        /// Registers a component as a singleton.
        /// </summary>
        /// <typeparam name="T">The type of the component to register.</typeparam>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public static IRegistrationBuilder RegisterSingleton<T>(this ContainerBuilder builder) => RegisterSingleton(builder, typeof(T));

        /// <summary>
        /// Registers a component as a singleton.
        /// </summary>
        /// <param name="type">The type of the component to register.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public static IRegistrationBuilder RegisterSingleton(this ContainerBuilder builder, Type type)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));

            return builder.Register(type, ComponentScope.Singleton);
        }

        /// <summary>
        /// Registers a component as a singleton.
        /// </summary>
        /// <typeparam name="T">The type of component to register.</typeparam>
        /// <param name="factory">A factory to use for constructing this component.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public static IRegistrationBuilder RegisterSingleton<T>(this ContainerBuilder builder, Func<ILifetimeScope, T> factory) where T : class
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return builder.Register(ComponentScope.Singleton, factory);
        }

        /// <summary>
        /// Registers a component as a singleton.
        /// </summary>
        /// <param name="type">The type of the component to register.</param>
        /// <param name="factory">A factory to use for constructing this component.</param>
        /// <remarks>No registration occurs by calling this method, the component must be registered using the returned builder.</remarks>
        public static IRegistrationBuilder RegisterSingleton(this ContainerBuilder builder, Type type, Func<ILifetimeScope, object> factory)
        {
            if (builder is null) throw new ArgumentNullException(nameof(builder));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            return builder.Register(type, ComponentScope.Singleton, factory);
        }

        #endregion Singleton
    }
}