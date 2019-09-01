using System;

namespace CoreMVVM.IOC
{
    /// <summary>
    /// Occurs when resolving a scoped component as <see cref="IOwned{T}"/>.
    /// </summary>
    public class OwnedScopedComponentException : Exception
    {
        public OwnedScopedComponentException(string message) : base(message)
        {
        }
    }
}