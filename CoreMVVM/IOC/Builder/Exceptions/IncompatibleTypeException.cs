using System;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// Thrown when a type is incompatible with the requested operation.
    /// </summary>
    public class IncompatibleTypeException : InvalidOperationException
    {
        public IncompatibleTypeException()
        {
        }

        public IncompatibleTypeException(string message) : base(message)
        {
        }

        public IncompatibleTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}