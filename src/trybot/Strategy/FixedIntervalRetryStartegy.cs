using System;

namespace Trybot.Strategy
{
    public class FixedIntervalRetryStartegy : RetryStartegy
    {
        public FixedIntervalRetryStartegy(int retryCount, TimeSpan delay)
            : base(retryCount, delay)
        {
        }

        protected override TimeSpan GetNextDelay(int currentAttempt)
        {
            return base.Delay;
        }
    }
}
