namespace CoreMVVM.IOC
{
    /// <summary>
    /// Occurs when the attempting to resolve an interface with the resolver.
    /// </summary>
    public sealed class ResolveUnregisteredInterfaceException : IOCException
    {
        public ResolveUnregisteredInterfaceException(string message) : base(message)
        {
        }
    }
}