using Ronin.Common;
using System;

namespace Trybot.Strategy
{
    public class FixedIntervalRetryStartegy : RetryStartegy
    {
        public FixedIntervalRetryStartegy(int retryCount, TimeSpan delay)
            : base(retryCount, delay)
        {
            Shield.EnsureTrue(retryCount > 0);
            Shield.EnsureTrue(delay > TimeSpan.FromMilliseconds(0));
        }

        protected override TimeSpan GetNextDelayInMilliseconds(int counter)
        {
            return base.Delay;
        }
    }
}
