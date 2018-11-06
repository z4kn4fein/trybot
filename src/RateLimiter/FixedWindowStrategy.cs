using System;
using System.Threading;

namespace Trybot.RateLimiter
{
    internal class FixedWindowStrategy : RateLimiterStrategy
    {
        public FixedWindowStrategy(int maxOperationCount, TimeSpan interval) : base(maxOperationCount, interval)
        { }

        public override bool ShouldLimit()
        {
            return false;
        }
    }
}
