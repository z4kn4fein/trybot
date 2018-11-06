using System;
using Trybot.Utils;

namespace Trybot.RateLimiter
{
    internal class FixedWindowStrategy : RateLimiterStrategy
    {
        private FixedTimeWindow window = FixedTimeWindow.New(DateTimeOffset.MinValue, 0);

        public FixedWindowStrategy(int maxOperationCount, TimeSpan interval) : base(maxOperationCount, interval)
        { }

        public override bool ShouldLimit()
        {
            Swap.SwapValue(ref this.window, (w, count) =>
                w.ExpirationTime < DateTimeOffset.UtcNow
                    ? FixedTimeWindow.New(DateTimeOffset.UtcNow, 1)
                    : FixedTimeWindow.New(w.ExpirationTime, w.OperationCount + 1),
            base.Interval);

            return this.window.OperationCount > base.MaxOperationCount;
        }

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
