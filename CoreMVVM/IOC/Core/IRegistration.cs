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
        /// Gets or sets the scope of this registration.
        /// </summary>
        InstanceScope Scope { get; set; }

        /// <summary>
        /// Gets or sets the factory of the registration.
        /// </summary>
        Func<ILifetimeScope, object> Factory { get; set; }
    }
}