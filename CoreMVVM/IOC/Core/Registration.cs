using System;

namespace CoreMVVM.IOC.Core
{
    /// <summary>
    /// Stores info about a registration.
    /// </summary>
    internal class Registration : IRegistration
    {
        public Registration(Type type, ComponentScope scope)
        {
            Type = type;
            Scope = scope;
        }

        public Type Type { get; }

        public ComponentScope Scope { get; }

        public Func<ILifetimeScope, object> Factory { get; set; }
    }
}