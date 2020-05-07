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

        public Type Type { get; }

        /// <summary>
        /// Gets or sets the scope of this registration.
        /// </summary>
        /// <value>Default is <see cref="ComponentScope.None"/>.</value>
        public ComponentScope Scope { get; set; } = ComponentScope.None;

        public Func<ILifetimeScope, object> Factory { get; set; }
    }
}