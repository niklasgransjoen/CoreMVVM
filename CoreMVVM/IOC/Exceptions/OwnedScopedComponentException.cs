﻿namespace CoreMVVM.IOC
{
    /// <summary>
    /// Occurs when resolving a scoped component as <see cref="IOwned{T}"/>.
    /// </summary>
    public class OwnedScopedComponentException : IOCException
    {
        public OwnedScopedComponentException()
        {
        }

        public OwnedScopedComponentException(string message) : base(message)
        {
        }

        public OwnedScopedComponentException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}