using System;

namespace Trybot.Retry.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an exception which will be thrown by the <see cref="IBotPolicy" /> when a retry bot reaches its configured maximum retry attempts.
    /// </summary>
    public class MaxRetryAttemptsReachedException : Exception
    {
        /// <summary>
        /// The result of the retried operation.
        /// </summary>
        public object OperationResult { get; set; }

        internal MaxRetryAttemptsReachedException(string message, Exception innerException, object result) : base(message, innerException)
        {
            this.OperationResult = result;
        }
    }
}
