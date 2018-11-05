using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trybot.RateLimiter
{
    internal class SlidingWindowStrategy : RateLimiterStrategy
    {
        public SlidingWindowStrategy(int maxOperationCount, TimeSpan interval) : base(maxOperationCount, interval)
        { }
    }
}
