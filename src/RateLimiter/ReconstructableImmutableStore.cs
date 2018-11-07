using System;

namespace Trybot.RateLimiter
{
    internal class ReconstructableImmutableStore<TData>
    {
        public static ReconstructableImmutableStore<TData> Empty = new ReconstructableImmutableStore<TData>();

        private readonly ReconstructableImmutableStore<TData> rest;

        public int Count { get; }

        public TData Data { get; }

        private ReconstructableImmutableStore() { }

        public ReconstructableImmutableStore(TData data, ReconstructableImmutableStore<TData> rest)
        {
            this.Data = data;
            this.rest = rest;
            this.Count = rest.Count + 1;
        }

        public ReconstructableImmutableStore<TData> Put(TData data) => new ReconstructableImmutableStore<TData>(data, this);

        public ReconstructableImmutableStore<TData> RebuildUntil(Func<TData, bool> predicate) =>
            this.RebuildUntilInternal(predicate);

        private ReconstructableImmutableStore<TData> RebuildUntilInternal(Func<TData, bool> predicate)
        {
            if (this == Empty || !predicate(this.Data))
                return Empty;

            return new ReconstructableImmutableStore<TData>(this.Data, this.rest.RebuildUntilInternal(predicate));
        }
    }
}
