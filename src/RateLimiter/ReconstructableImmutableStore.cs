using System;

namespace Trybot.RateLimiter
{
    internal class ReconstructableImmutableStore<TData>
    {
        public static ReconstructableImmutableStore<TData> Empty = new ReconstructableImmutableStore<TData>();

        private readonly ReconstructableImmutableStore<TData> rest;

        public ReconstructableImmutableStore<TData> Last { get; }

        public int Count { get; }

        public TData Data { get; }

        private ReconstructableImmutableStore() { }

        private ReconstructableImmutableStore(TData data, ReconstructableImmutableStore<TData> last, ReconstructableImmutableStore<TData> rest)
            : this(data, rest)
        {
            this.Last = last;
        }

        private ReconstructableImmutableStore(TData data, ReconstructableImmutableStore<TData> rest)
        {
            this.Data = data;
            this.Last = this;
            this.rest = rest;
            this.Count = rest.Count + 1;
        }

        public ReconstructableImmutableStore<TData> Put(TData data)
        {
            return this == Empty
                ? new ReconstructableImmutableStore<TData>(data, this)
                : new ReconstructableImmutableStore<TData>(data, this.Last, this);
        }

        public ReconstructableImmutableStore<TData> RebuildUntil(Func<TData, bool> predicate) =>
            this.RebuildUntilInternal(predicate);

        private ReconstructableImmutableStore<TData> RebuildUntilInternal(Func<TData, bool> predicate)
        {
            if (this == Empty || !predicate(this.Data))
                return Empty;

            var next = this.rest.RebuildUntilInternal(predicate);
            return next == Empty
                ? new ReconstructableImmutableStore<TData>(this.Data, this, next)
                : new ReconstructableImmutableStore<TData>(this.Data, next.Last, next);
        }
    }
}
