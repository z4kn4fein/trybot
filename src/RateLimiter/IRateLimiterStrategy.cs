using System;

namespace Trybot.RateLimiter
{
    /// <summary>
    /// Represent a rate limiter strategy interface.
    /// </summary>
    public interface IRateLimiterStrategy
    {
        /// <summary>
        /// Determines whether the current operation should be rejected by the rate limirer strategy or not.
        /// </summary>
        /// <param name="retryAfter">The time after the caller should retry the given operation.</param>
        /// <returns>True if the current operation should be rejected, otherwise false.</returns>
        bool ShouldLimit(out TimeSpan retryAfter);
    }
}
