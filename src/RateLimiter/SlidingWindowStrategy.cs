using System;
using Trybot.Utils;

namespace Trybot.RateLimiter
{
    internal class SlidingWindowStrategy : RateLimiterStrategy
    {
        private ReconstructableImmutableStore<DateTimeOffset> timeHistory;

        public SlidingWindowStrategy(int maxOperationCount, TimeSpan interval) : base(maxOperationCount, interval)
        {
            this.timeHistory = ReconstructableImmutableStore<DateTimeOffset>.Empty;
        }

        public override bool ShouldLimit()
        {
            Swap.SwapValue(ref this.timeHistory, (store, count) =>
            {
                var rebuilt = store.RebuildUntil(time => time >= DateTimeOffset.UtcNow);
                return rebuilt.Count >= count ? rebuilt : rebuilt.Put(DateTimeOffset.UtcNow.Add(base.Interval));
            }, base.MaxOperationCount);

            return this.timeHistory.Count > MaxOperationCount;
        }
    }
}
