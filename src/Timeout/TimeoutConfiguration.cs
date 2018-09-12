using System;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot.Timeout
{
    public class TimeoutConfiguration
    {
        internal TimeSpan Timeout { get; set; }

        private Action<ExecutionContext> timeoutHandler;
        private Func<ExecutionContext, Task> asyncTimeoutHandler;

        public TimeoutConfiguration After(TimeSpan timeout)
        {
            this.Timeout = timeout;
            return this;
        }

        public TimeoutConfiguration OnTimeout(Action<ExecutionContext> timeoutHandler)
        {
            Shield.EnsureNotNull(timeoutHandler, nameof(timeoutHandler));

            this.timeoutHandler = timeoutHandler;
            return this;
        }

        public TimeoutConfiguration OnTimeoutAsync(Func<ExecutionContext, Task> asyncTimeoutHandler)
        {
            Shield.EnsureNotNull(asyncTimeoutHandler, nameof(asyncTimeoutHandler));

            this.asyncTimeoutHandler = asyncTimeoutHandler;
            return this;
        }

        internal void RaiseTimeoutEvent(ExecutionContext context) =>
            this.timeoutHandler?.Invoke(context);

        internal async Task RaiseAsyncTimeoutEvent(ExecutionContext context)
        {
            this.timeoutHandler?.Invoke(context);

            if (this.asyncTimeoutHandler == null)
                return;

            await this.asyncTimeoutHandler(context)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
        }

    }
}