namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// Thrown when a generic type definition is incompatible with the requested operation.
    /// </summary>
    public class IncompatibleGenericTypeDefinitionException : IncompatibleTypeException
    {
        public IncompatibleGenericTypeDefinitionException(string message) : base(message)
        {
        }
    }
}