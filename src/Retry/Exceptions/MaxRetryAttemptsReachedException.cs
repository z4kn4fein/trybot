using System;

namespace Trybot.Retry.Exceptions
{
    public class MaxRetryAttemptsReachedException : Exception
    {
        public object OperationResult { get; set; }

        public MaxRetryAttemptsReachedException(string message, Exception innerException, object result) : base(message, innerException)
        {
            this.OperationResult = result;
        }
    }
}
