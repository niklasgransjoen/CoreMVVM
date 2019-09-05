using System;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// Thrown when a type is incompatible with the requested operation.
    /// </summary>
    public class IncompatibleTypeException : InvalidOperationException
    {
        public IncompatibleTypeException(string message) : base(message)
        {
        }
    }
}