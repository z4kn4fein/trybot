using System;

namespace Trybot.RateLimiter.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an exception thrown by the <see cref="IBotPolicy" /> when a configured rate limiter bot rejects an operation.
    /// </summary>
    public class RateLimitExceededException : Exception
    {
        /// <inheritdoc />
        public RateLimitExceededException(string message) : base(message)
        {
        }
    }
}
