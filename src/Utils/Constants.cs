using System.Threading.Tasks;

namespace Trybot.Utils
{
    internal static class Constants
    {
        public const string MaxRetryExceptionMessage = "The maximum number of retry attempts reached.";

        public const string RateLimitExceededExceptionMessage = "The maximum number of allowed operations within the given time interval reached. Retry after: {0}";

        public const string TimeoutExceptionMessage = "The operation cancelled by TimeoutBot.";

        public const string CircuitOpenExceptionMessage = "The circuit is in Open state!";

        public const string HalfOpenExecutionLimitExceededExceptionMessage = "The circuit in HalfOpen state allows only one operation to execute at the same time.";

        public static readonly Task<int> CompletedTask = Task.FromResult(0);

        public static readonly int DummyReturnValue = 0;
    }
}
