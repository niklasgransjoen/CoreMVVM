using System;

namespace CoreMVVM.IOC
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