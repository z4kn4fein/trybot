using System;

namespace Trybot.Interfaces
{
    /// <summary>
    /// Interface for retry policy implementations.
    /// </summary>
    public interface IRetryPolicy
    {
        /// <summary>
        /// Determines on which exception should the <see cref="IRetryManager"/> retry the related operation. 
        /// </summary>
        /// <param name="exception">The throwed exception.</param>
        /// <returns>True if the <see cref="IRetryManager"/> should retry the operation, otherwise false.</returns>
        bool ShouldRetryAfter(Exception exception);
    }
}
