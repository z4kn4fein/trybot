using System;

namespace Trybot.Strategy
{
    public class CubicRetryStrategy : RetryStartegy
    {
        public CubicRetryStrategy(int retryCount, TimeSpan delay)
            : base(retryCount, delay)
        {
        }

        protected override TimeSpan GetNextDelay(int currentAttempt)
        {
            var tmpDelay = currentAttempt * base.Delay.TotalMilliseconds;
            return TimeSpan.FromMilliseconds(tmpDelay * tmpDelay * tmpDelay);
        }
    }
}
