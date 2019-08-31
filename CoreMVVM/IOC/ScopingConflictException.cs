using System;

namespace CoreMVVM.IOC
{
    /// <summary>
    /// Occurs when attempting to register the same type multiple times, with different scopes.
    /// </summary>
    public class ScopingConflictException : Exception
    {
        public ScopingConflictException(string message) : base(message)
        {
        }
    }
}