using System;
using System.Collections;
using System.Collections.Generic;

namespace Trybot.RateLimiter
{
    internal class SlidingStore<TData> : IEnumerable<TData>
    {
        public static SlidingStore<TData> Empty = new SlidingStore<TData>(default, null);

        private readonly TData data;
        private readonly SlidingStore<TData> rest;

        public SlidingStore(TData data, SlidingStore<TData> rest)
        {
            this.data = data;
            this.rest = rest;
        }

        public SlidingStore<TData> Put(TData data) => new SlidingStore<TData>(data, this);

        public SlidingStore<TData> RebuildUntil(Func<TData, bool> predicate) =>
            this.RebuildUntilInternal(predicate);

        private SlidingStore<TData> RebuildUntilInternal(Func<TData, bool> predicate)
        {
            if (this == Empty || !predicate(this.data))
                return Empty;

            return new SlidingStore<TData>(this.data,  this.rest.RebuildUntilInternal(predicate));
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
