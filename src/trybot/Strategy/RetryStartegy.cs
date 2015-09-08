using Ronin.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Strategy
{
    public abstract class RetryStartegy
    {
        public static RetryStartegy GetDefaultPolicy()
        {
            return new FixedIntervalRetryStartegy(5, TimeSpan.FromMilliseconds(500));
        }

        public int Counter { get; private set; }
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

        public bool IsCompleted()
        {
            return this.Counter >= this.RetryCount;
        }

        public async Task WaitAsync(CancellationToken token)
        {
            this.NextDelay = GetNextDelayInMilliseconds(++this.Counter);
            await TaskDelayer.Sleep(this.NextDelay, token);
        }

        protected abstract TimeSpan GetNextDelayInMilliseconds(int counter);
    }
}
