using System;

namespace CoreMVVM.IOC.Core
{
    /// <summary>
    /// Occurs when the attempting to resolve an interface with the resolver.
    /// </summary>
    public class ResolveUnregisteredInterfaceException : Exception
    {
        public ResolveUnregisteredInterfaceException(string message) : base(message)
        {
        }
    }
}