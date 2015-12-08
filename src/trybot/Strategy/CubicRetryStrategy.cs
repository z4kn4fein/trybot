using Ronin.Common;
using System;

namespace Trybot.Strategy
{
    /// <summary>
    /// Represents a retry strategy with basic cubic delay calculation.
    /// </summary>
    public class CubicRetryStrategy : RetryStartegy
    {
        private TimeSpan tmpDelay;

        /// <summary>
        /// Constructs a <see cref="CubicRetryStrategy"/>
        /// </summary>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="delay">The initial delay.</param>
        public CubicRetryStrategy(int retryCount, TimeSpan delay)
            : base(retryCount, delay)
        {
            Shield.EnsureTrue(retryCount > 0);
            Shield.EnsureTrue(delay > TimeSpan.FromMilliseconds(0));

            this.tmpDelay = delay;
        }


        protected override TimeSpan GetNextDelay(int counter)
        {
            return this.tmpDelay = TimeSpan.FromMilliseconds(this.tmpDelay.TotalMilliseconds * this.tmpDelay.TotalMilliseconds * this.tmpDelay.TotalMilliseconds);
        }
    }
}
