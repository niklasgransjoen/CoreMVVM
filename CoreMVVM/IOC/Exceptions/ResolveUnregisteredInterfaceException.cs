namespace CoreMVVM.IOC
{
    /// <summary>
    /// Occurs when the attempting to resolve an unregistered service.
    /// </summary>
    public sealed class ResolveUnregisteredServiceException : IOCException
    {
        public ResolveUnregisteredServiceException()
        {
        }

        public ResolveUnregisteredServiceException(string message) : base(message)
        {
        }

        public ResolveUnregisteredServiceException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}