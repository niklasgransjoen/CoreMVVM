using System;

namespace CoreMVVM.IOC.Builder
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
        /// Gets or sets a value indicating if this registration should only be created once.
        /// </summary>
        public bool IsSingleton { get; set; }

        /// <summary>
        /// Gets or sets the last created instance of this registration.
        /// </summary>
        public object SingletonInstance { get; set; }

        /// <summary>
        /// Gets or sets the factory of the registration.
        /// </summary>
        public Func<ILifetimeScope, object> Factory { get; set; }
    }
}