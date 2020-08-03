using System;

namespace CoreMVVM.IOC.Core
{
    internal interface IRegistration
    {
        /// <summary>
        /// Gets the type of this registration.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets the scope of this registration.
        /// </summary>
        ComponentScope Scope { get; }

        /// <summary>
        /// Gets or sets the factory of the registration.
        /// </summary>
        Func<ILifetimeScope, object>? Factory { get; set; }
    }
}