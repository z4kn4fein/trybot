using System;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot.Timeout
{
    public class TimeoutConfiguration
    {
        internal TimeSpan Timeout { get; set; }

        internal Action<ExecutionContext> TimeoutHandler { get; set; }

        internal Func<ExecutionContext, Task> AsyncTimeoutHandler { get; set; }

        public TimeoutConfiguration After(TimeSpan timeout)
        {
            this.Timeout = timeout;
            return this;
        }

        public TimeoutConfiguration OnTimeout(Action<ExecutionContext> timeoutHandler)
        {
            Shield.EnsureNotNull(timeoutHandler, nameof(timeoutHandler));

            this.TimeoutHandler = timeoutHandler;
            return this;
        }

        public TimeoutConfiguration OnTimeoutAsync(Func<ExecutionContext, Task> asyncTimeoutHandler)
        {
            Shield.EnsureNotNull(asyncTimeoutHandler, nameof(asyncTimeoutHandler));

            this.AsyncTimeoutHandler = asyncTimeoutHandler;
            return this;
        }
    }
}