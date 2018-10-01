using System;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot.Timeout
{
    /// <summary>
    /// Represents the configuration of the timeout bot.
    /// </summary>
    public class TimeoutConfiguration
    {
        internal TimeSpan Timeout { get; set; }

        private Action<ExecutionContext> onTimeout;

        private Func<ExecutionContext, Task> onTimeoutAsync;

        /// <summary>
        /// Sets after how much time should be the given operation cancelled.
        /// </summary>
        /// <param name="timeout">The timeout value.</param>
        /// <returns>Itself because of the fluent access.</returns>
        public TimeoutConfiguration After(TimeSpan timeout)
        {
            this.Timeout = timeout;
            return this;
        }

        /// <summary>
        /// Sets the delegate which will be invoked when the given operation is timing out.
        /// </summary>
        /// <param name="timeoutHandler">The action to be invoked on a timeout.</param>
        /// <returns>Itself because of the fluent access.</returns>
        /// <example><code>config.OnTimeout(context => Console.WriteLine("Operation timed out."))</code></example>
        public TimeoutConfiguration OnTimeout(Action<ExecutionContext> timeoutHandler)
        {
            Shield.EnsureNotNull(timeoutHandler, nameof(timeoutHandler));

            this.onTimeout = timeoutHandler;
            return this;
        }

        /// <summary>
        /// Sets the asynchronous delegate which will be invoked when the given operation is timing out.
        /// </summary>
        /// <param name="asyncTimeoutHandler">The asynchronous action to be invoked on a timeout.</param>
        /// <returns>Itself because of the fluent access.</returns>
        /// <example><code>config.OnTimeoutAsync(async context => await onTimeoutActionAsync())</code></example>
        public TimeoutConfiguration OnTimeoutAsync(Func<ExecutionContext, Task> asyncTimeoutHandler)
        {
            Shield.EnsureNotNull(asyncTimeoutHandler, nameof(asyncTimeoutHandler));

            this.onTimeoutAsync = asyncTimeoutHandler;
            return this;
        }

        internal void RaiseTimeoutEvent(ExecutionContext context) =>
            this.onTimeout?.Invoke(context);

        internal async Task RaiseAsyncTimeoutEvent(ExecutionContext context)
        {
            this.onTimeout?.Invoke(context);

            if (this.onTimeoutAsync == null)
                return;

            await this.onTimeoutAsync(context)
                .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
        }

    }
}