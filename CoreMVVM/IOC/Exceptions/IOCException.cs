using System;
using System.Runtime.Serialization;

namespace CoreMVVM.IOC
{
    public abstract class IOCException : Exception
    {
        public IOCException()
        {
        }

        public IOCException(string message) : base(message)
        {
        }

        public IOCException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IOCException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}