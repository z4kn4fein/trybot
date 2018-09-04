using System;

namespace Trybot.Retry.Model
{
    public class AttemptContext
    {
        internal static AttemptContext New(int currentAttempt, TimeSpan currentDelay, ExecutionContext executionContext) =>
            new AttemptContext(currentAttempt, currentDelay, executionContext);

        public int CurrentAttempt { get; }

        public TimeSpan CurrentDelay { get; }

        public ExecutionContext ExecutionContext { get; }

        private AttemptContext(int currentAttempt, TimeSpan currentDelay, ExecutionContext executionContext)
        {
            this.CurrentAttempt = currentAttempt;
            this.CurrentDelay = currentDelay;
            this.ExecutionContext = executionContext;
        }
    }
}
