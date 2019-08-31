using CoreMVVM.IOC.Builder;
using System;
using System.Collections.Generic;

namespace CoreMVVM.IOC.Core
{
    /// <summary>
    /// Implementation of <see cref="IContainer"/>.
    /// </summary>
    internal sealed class Container : LifetimeScope, IContainer
    {
        /// <summary>
        /// Creates a new container.
        /// </summary>
        /// <param name="registeredTypes">The registered types of this container.</param>
        public Container(IReadOnlyDictionary<Type, IRegistration> registeredTypes) : base(registeredTypes)
        {
        }
    }
}