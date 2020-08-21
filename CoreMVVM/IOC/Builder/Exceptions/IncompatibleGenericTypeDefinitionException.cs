namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// Thrown when a generic type definition is incompatible with the requested operation.
    /// </summary>
    public class IncompatibleGenericTypeDefinitionException : IncompatibleTypeException
    {
        public IncompatibleGenericTypeDefinitionException()
        {
        }

        public IncompatibleGenericTypeDefinitionException(string message) : base(message)
        {
        }

        public IncompatibleGenericTypeDefinitionException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}