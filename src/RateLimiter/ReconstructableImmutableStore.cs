using System;
using System.Collections;
using System.Collections.Generic;

namespace Trybot.RateLimiter
{
    internal class ReconstructableImmutableStore<TData> : IEnumerable<TData>
    {
        public static ReconstructableImmutableStore<TData> Empty = new ReconstructableImmutableStore<TData>(default, null);

        private readonly TData data;
        private readonly ReconstructableImmutableStore<TData> rest;

        public int Count { get; }

        public ReconstructableImmutableStore(TData data, ReconstructableImmutableStore<TData> rest)
        {
            this.data = data;
            this.rest = rest;
            this.Count = rest.Count + 1;
        }

        public ReconstructableImmutableStore<TData> Put(TData data) => new ReconstructableImmutableStore<TData>(data, this);

        public ReconstructableImmutableStore<TData> RebuildUntil(Func<TData, bool> predicate) =>
            this.RebuildUntilInternal(predicate);

        private ReconstructableImmutableStore<TData> RebuildUntilInternal(Func<TData, bool> predicate)
        {
            if (this == Empty || !predicate(this.data))
                return Empty;

            return new ReconstructableImmutableStore<TData>(this.data, this.rest.RebuildUntilInternal(predicate));
        }

        public IEnumerator<TData> GetEnumerator()
        {
            for (var current = this; current != Empty; current = current.rest)
                yield return current.data;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
