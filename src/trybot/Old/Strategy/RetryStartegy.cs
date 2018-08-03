using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot.Strategy
{
    /// <summary>
    /// Base class for RetryStrategy implementations.
    /// </summary>
    public abstract class RetryStartegy
    {
        private static RetryStartegy defaultRetryStrategy = new FixedIntervalRetryStartegy(5, TimeSpan.FromMilliseconds(500));

        /// <summary>
        /// Gets or sets the default retry strategy.
        /// </summary>
        public static RetryStartegy DefaultRetryStrategy
        {
            get => defaultRetryStrategy;
            set { Shield.EnsureNotNull(value, nameof(value)); defaultRetryStrategy = value; }
        }

        /// <summary>
        /// The current attempt value.
        /// </summary>
        public int CurrentAttempt { get; private set; }

        /// <summary>
        /// The next delay value.
        /// </summary>
        public TimeSpan NextDelay { get; private set; }

        /// <summary>
        /// The retry count.
        /// </summary>
        protected readonly int RetryCount;

        /// <summary>
        /// The initial delay value.
        /// </summary>
        protected readonly TimeSpan Delay;

        /// <summary>
        /// Base constructor of <see cref="RetryStartegy"/> implementations.
        /// </summary>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="delay">The initial delay.</param>
        protected RetryStartegy(int retryCount, TimeSpan delay)
        {
            Shield.EnsureTrue(retryCount > 0);
            Shield.EnsureTrue(delay > TimeSpan.FromMilliseconds(0));

            this.RetryCount = retryCount;
            this.Delay = delay;
        }

        internal bool IsCompleted() =>
            this.CurrentAttempt >= this.RetryCount;

        internal async Task WaitAsync(CancellationToken token) =>
            await TaskDelayer.Sleep(this.NextDelay, token);

        internal void CalculateNextDelay() =>
            this.NextDelay = GetNextDelay(++this.CurrentAttempt);

        /// <summary>
        /// Calculates the next delay value.
        /// </summary>
        /// <param name="currentAttempt">The current attempt.</param>
        /// <returns>The next delay value.</returns>
        protected abstract TimeSpan GetNextDelay(int currentAttempt);
    }
}
