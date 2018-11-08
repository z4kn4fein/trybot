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

        public override bool ShouldLimit(out TimeSpan retryAfter)
        {
            var result = Swap.SwapValue(ref this.timeHistory, store => this.Refresh(store));

            retryAfter = this.timeHistory.Last.Data - DateTimeOffset.UtcNow;
            return result;
        }

        private Tuple<ReconstructableImmutableStore<DateTimeOffset>, bool> Refresh(ReconstructableImmutableStore<DateTimeOffset> previous)
        {
            var rebuilt = previous.RebuildUntil(time => time >= DateTimeOffset.UtcNow);
            return rebuilt.Count >= base.MaxOperationCount
                ? Tuple.Create(rebuilt, true)
                : Tuple.Create(rebuilt.Put(DateTimeOffset.UtcNow.Add(base.Interval)), false);
        }
    }
}
