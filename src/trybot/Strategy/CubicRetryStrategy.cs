﻿using Ronin.Common;
using System;

namespace Trybot.Strategy
{
    public class CubicRetryStrategy : RetryStartegy
    {
        private TimeSpan tmpDelay;

        public CubicRetryStrategy(int retryCount, TimeSpan delay)
            : base(retryCount, delay)
        {
            Shield.EnsureTrue(retryCount > 0);
            Shield.EnsureTrue(delay > TimeSpan.FromMilliseconds(0));

            this.tmpDelay = delay;
        }

        protected override TimeSpan GetNextDelay(int counter)
        {
            return this.tmpDelay = TimeSpan.FromMilliseconds(this.tmpDelay.TotalMilliseconds * this.tmpDelay.TotalMilliseconds * this.tmpDelay.TotalMilliseconds);
        }
    }
}
