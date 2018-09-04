using System;

namespace Trybot.Retry.Exceptions
{
    public class MaxRetryAttemptsReachedException<TResult> : Exception
    {
        public TResult OperationResult { get; set; }

        public MaxRetryAttemptsReachedException(string message, Exception innerException, TResult result) : base(message, innerException)
        {
            this.OperationResult = result;
        }
    }

    public class MaxRetryAttemptsReachedException : Exception
    {
        public MaxRetryAttemptsReachedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
