using System;
using Trybot.Utils;

namespace Trybot.RateLimiter
{
    internal class SlidingWindowStrategy : RateLimiterStrategy
    {
        private SlidingStore<DateTimeOffset> timeHistory;

        public SlidingWindowStrategy(int maxOperationCount, TimeSpan interval) : base(maxOperationCount, interval)
        {
            this.timeHistory = SlidingStore<DateTimeOffset>.Empty;
        }

        public override bool ShouldLimit()
        {
            Swap.SwapValue(ref this.timeHistory, (store, count) =>
            {
                var rebuilt = store.RebuildUntil(time => time >= DateTimeOffset.UtcNow.Add(-base.Interval));
                if (rebuilt.Count >= count)
                    return rebuilt;

                return rebuilt.Put(DateTimeOffset.UtcNow);
            }, base.MaxOperationCount);

            return this.timeHistory.Count >= MaxOperationCount;
        }
    }
}
