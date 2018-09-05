using System;

namespace Trybot.Timeout.Exceptions
{
    public class OperationTimeoutException : Exception
    {
        public OperationTimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
