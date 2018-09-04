using System;
using System.Threading.Tasks;

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
            this.TimeoutHandler = timeoutHandler;
            return this;
        }

        public TimeoutConfiguration OnTimeout(Func<ExecutionContext, Task> asyncTimeoutHandler)
        {
            this.AsyncTimeoutHandler = asyncTimeoutHandler;
            return this;
        }
    }
}