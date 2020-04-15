using System;

namespace CoreMVVM.IOC
{
    /// <summary>
    /// Occurs when attempting to register the same type multiple times, with different scopes.
    /// </summary>
    public sealed class ScopingConflictException : IOCException
    {
        public ScopingConflictException(Type componentType, Type serviceType, ComponentScope currentScope, ComponentScope previousScope)
        {
            Message =
                $"Attempted to register type '{componentType}' with scope '{currentScope}', " +
                $"which conflicts with earlier registration as a component of '{serviceType}', with scope '{previousScope}'.";
        }

        public override string Message { get; }
    }
}