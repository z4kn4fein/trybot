using System;

namespace Trybot.RateLimiter.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an exception thrown by the <see cref="IBotPolicy" /> when a configured rate limiter bot rejects an operation.
    /// </summary>
    public class RateLimitExceededException : Exception
    {
        /// <summary>
        /// The time after the caller should retry the given operation.
        /// </summary>
        public TimeSpan RetryAfter { get; }

        /// <inheritdoc />
        public RateLimitExceededException(string message, TimeSpan retryAfter) : base(string.Format(message, retryAfter))
        {
            this.RetryAfter = retryAfter;
        }
    }
}
