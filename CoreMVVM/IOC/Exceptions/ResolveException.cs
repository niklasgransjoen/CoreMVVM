using System;

namespace CoreMVVM.IOC
{
    /// <summary>
    /// Occurs when resolving a service fails.
    /// </summary>
    public sealed class ResolveException : IOCException
    {
        public ResolveException()
        {
        }

        public ResolveException(string message) : base(message)
        {
        }

        public ResolveException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}