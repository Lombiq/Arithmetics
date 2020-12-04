using System;
using System.Runtime.Serialization;

namespace Lombiq.Arithmetics
{
    [Serializable]
    public class UnumException : Exception
    {
        public UnumException(string message)
            : base(message) { }

        public UnumException(string message, Exception innerException)
            : base(message, innerException) { }

        public UnumException() { }

        protected UnumException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) { }
    }
}
