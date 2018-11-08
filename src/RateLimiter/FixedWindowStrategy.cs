using System;
using Trybot.Utils;

namespace Trybot.RateLimiter
{
    internal class FixedWindowStrategy : RateLimiterStrategy
    {
        private FixedTimeWindow window = FixedTimeWindow.New(DateTimeOffset.MinValue, 0);

        public FixedWindowStrategy(int maxOperationCount, TimeSpan interval) : base(maxOperationCount, interval)
        { }

        public override bool ShouldLimit(out TimeSpan retryAfter)
        {
            Swap.SwapValue(ref this.window, this.Refresh);

            retryAfter = this.window.ExpirationTime - DateTimeOffset.UtcNow;
            return this.window.OperationCount > base.MaxOperationCount;
        }

        private FixedTimeWindow Refresh(FixedTimeWindow previous) =>
            previous.ExpirationTime < DateTimeOffset.UtcNow
                ? FixedTimeWindow.New(DateTimeOffset.UtcNow.Add(base.Interval), 1)
                : previous.OperationCount > base.MaxOperationCount
                    ? previous
                    : FixedTimeWindow.New(previous.ExpirationTime, previous.OperationCount + 1);

        private class FixedTimeWindow
        {
            public static FixedTimeWindow New(DateTimeOffset expiration, int count)
                => new FixedTimeWindow(expiration, count);

            public DateTimeOffset ExpirationTime { get; }

            public int OperationCount { get; }

            private FixedTimeWindow(DateTimeOffset expirationTime, int count)
            {
                this.ExpirationTime = expirationTime;
                this.OperationCount = count;
            }
        }
    }
}
