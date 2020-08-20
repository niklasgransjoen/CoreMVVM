using CoreMVVM.IOC.Core;
using System;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// For performing component/service registration.
    /// </summary>
    internal class RegistrationBuilder : IRegistrationBuilder
    {
        private readonly ToolBox _toolBox;
        private readonly bool _overwriteFactory;

        #region Constructors

        public RegistrationBuilder(
            ToolBox toolBox,
            Type type,
            ComponentScope scope)
        {
            _toolBox = toolBox;
            Type = type;
            Scope = scope;

            // Factory is not explicitly set. Copy eventual factory from earlier registration.
            _overwriteFactory = false;
        }

        public RegistrationBuilder(
            ToolBox toolBox,
            Type type,
            ComponentScope scope,
            Func<ILifetimeScope, object> factory)
        {
            _toolBox = toolBox;

            Type = type;
            Scope = scope;
            Factory = factory;

            // Factory is explicitly set (even if it may be null), replace old factory if there's an earlier registration.
            _overwriteFactory = true;
        }

        #endregion Constructors

        #region Properties

        public Type Type { get; }

        public ComponentScope Scope { get; }

        public Func<ILifetimeScope, object>? Factory { get; }

        #endregion Properties

        #region Methods

        public IRegistrationBuilder As(Type serviceType)
        {
            if (_overwriteFactory)
                _toolBox.AddRegistration(Type, serviceType, Scope, Factory);
            else
                _toolBox.AddRegistration(Type, serviceType, Scope);

            return this;
        }

        #endregion Methods
    }

    internal class RegistrationBuilder<T> : RegistrationBuilder, IRegistrationBuilder<T>
        where T : class
    {
        public RegistrationBuilder(
            ToolBox toolBox,
            ComponentScope scope)
            : base(toolBox, typeof(T), scope)
        {
        }

        public RegistrationBuilder(
            ToolBox toolBox,
            ComponentScope scope,
            Func<ILifetimeScope, T> factory)
            : base(toolBox, typeof(T), scope, factory)
        {
        }
    }
}