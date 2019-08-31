using System;

namespace CoreMVVM.IOC.Builder
{
    internal interface IRegistration
    {
        /// <summary>
        /// Gets the type of this registration.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets or sets a value indicating if this registration should only be created once.
        /// </summary>
        bool IsSingleton { get; set; }

        /// <summary>
        /// Gets or sets the instance of this registration. Null if not created yet, or if not a singleton.
        /// </summary>
        object SingletonInstance { get; set; }

        /// <summary>
        /// Gets or sets the factory of the registration.
        /// </summary>
        Func<ILifetimeScope, object> Factory { get; set; }
    }
}