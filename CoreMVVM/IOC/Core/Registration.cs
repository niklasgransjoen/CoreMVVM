using System;

namespace CoreMVVM.IOC.Core
{
    /// <summary>
    /// Stores info about a registration.
    /// </summary>
    internal class Registration : IRegistration
    {
        public Registration(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the type of this registration.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets or sets the scope of this registration.
        /// </summary>
        /// <value>Default is <see cref="ComponentScope.None"/>.</value>
        public ComponentScope Scope { get; set; } = ComponentScope.None;

        /// <summary>
        /// Gets or sets the factory of the registration.
        /// </summary>
        public Func<ILifetimeScope, object> Factory { get; set; }
    }
}