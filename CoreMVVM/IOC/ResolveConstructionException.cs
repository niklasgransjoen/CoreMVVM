using System;

namespace CoreMVVM.IOC
{
    /// <summary>
    /// Occurs when construction fails during resolve.
    /// </summary>
    public class ResolveConstructionException : Exception
    {
        public ResolveConstructionException(string message) : base(message)
        {
        }

        public ResolveConstructionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}