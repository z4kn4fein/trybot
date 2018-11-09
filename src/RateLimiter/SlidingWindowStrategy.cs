using System;
using Trybot.Utils;

namespace Trybot.RateLimiter
{
    internal class SlidingWindowStrategy : IRateLimiterStrategy
    {
        private readonly int maxOperationCount;
        private readonly TimeSpan interval;
        private ReconstructableImmutableStore<DateTimeOffset> timeHistory;

        public SlidingWindowStrategy(int maxOperationCount, TimeSpan interval)
        {
            this.maxOperationCount = maxOperationCount;
            this.interval = interval;
            this.timeHistory = ReconstructableImmutableStore<DateTimeOffset>.Empty;
        }

        public bool ShouldLimit(out TimeSpan retryAfter)
        {
            var result = Swap.SwapValue(ref this.timeHistory, store => this.Refresh(store));

            retryAfter = this.timeHistory.Last.Data - DateTimeOffset.UtcNow;
            return result;
        }

        private Tuple<ReconstructableImmutableStore<DateTimeOffset>, bool> Refresh(ReconstructableImmutableStore<DateTimeOffset> previous)
        {
            var rebuilt = previous.RebuildUntil(time => time >= DateTimeOffset.UtcNow);
            return rebuilt.Count >= this.maxOperationCount
                ? Tuple.Create(rebuilt, true)
                : Tuple.Create(rebuilt.Put(DateTimeOffset.UtcNow.Add(this.interval)), false);
        }
    }
}
