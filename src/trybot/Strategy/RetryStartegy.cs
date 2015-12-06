using Ronin.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Strategy
{
    public abstract class RetryStartegy
    {
        private static RetryStartegy defaultRetryStrategy = new FixedIntervalRetryStartegy(5, TimeSpan.FromMilliseconds(500));
        public static RetryStartegy DefaultRetryStrategy
        {
            get { return defaultRetryStrategy; }
            set { if (value != null) defaultRetryStrategy = value; }
        }

        public int CurrentAttempt { get; private set; }
        public TimeSpan NextDelay { get; private set; }

        protected readonly int RetryCount;
        protected readonly TimeSpan Delay;

        protected RetryStartegy(int retryCount, TimeSpan delay)
        {
            Shield.EnsureTrue(retryCount > 0);
            Shield.EnsureTrue(delay > TimeSpan.FromMilliseconds(0));

            this.RetryCount = retryCount;
            this.Delay = delay;
        }

        internal bool IsCompleted()
        {
            return this.CurrentAttempt >= this.RetryCount;
        }

        internal async Task WaitAsync(CancellationToken token)
        {
            await TaskDelayer.Sleep(this.NextDelay, token);
        }

        internal void CalculateNextDelay()
        {
            this.NextDelay = GetNextDelay(++this.CurrentAttempt);
        }

        protected abstract TimeSpan GetNextDelay(int counter);
    }
}
