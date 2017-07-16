using System;

namespace Lombiq.Unum
{
    public class UnumException : Exception
    {
        public UnumException(string message) : base(message) { }

        public UnumException(string message, Exception innerException) : base(message, innerException) { }
    }
}
