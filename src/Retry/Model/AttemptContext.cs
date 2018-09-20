using System;

namespace Trybot.Retry.Model
{
    /// <summary>
    /// Represents a retry attempt context which holds additional information about the current retry operation.
    /// </summary>
    public class AttemptContext
    {
        internal static AttemptContext New(int currentAttempt, TimeSpan currentDelay, ExecutionContext executionContext) =>
            new AttemptContext(currentAttempt, currentDelay, executionContext);

        /// <summary>
        /// The current attempt count.
        /// </summary>
        public int CurrentAttempt { get; }

        /// <summary>
        /// The amount of the current delay.
        /// </summary>
        public TimeSpan CurrentDelay { get; }

        /// <summary>
        /// The <see cref="ExecutionContext"/> of the current execution.
        /// </summary>
        public ExecutionContext ExecutionContext { get; }

        private AttemptContext(int currentAttempt, TimeSpan currentDelay, ExecutionContext executionContext)
        {
            this.CurrentAttempt = currentAttempt;
            this.CurrentDelay = currentDelay;
            this.ExecutionContext = executionContext;
        }
    }
}
