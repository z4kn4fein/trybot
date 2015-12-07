using System;

namespace Trybot.Strategy
{
    public class LinearRetryStrategy : RetryStartegy
    {
        public LinearRetryStrategy(int retryCount, TimeSpan delay)
            : base(retryCount, delay)
        {
        }

        protected override TimeSpan GetNextDelay(int currentAttempt)
        {
            return TimeSpan.FromMilliseconds(currentAttempt * base.Delay.TotalMilliseconds);
        }
    }
}
