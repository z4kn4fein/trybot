using System.Threading.Tasks;

namespace Trybot.Utils
{
    internal static class Constants
    {
        public const string MaxRetryExceptionMessage = "Maximum number of retry attempts reached.";

        public const string TimeoutExceptionMessage = "The operation cancelled by TimeoutBot.";

        public static readonly Task<int> CompletedTask = Task.FromResult(0);

        public static readonly int DummyReturnValue = 0;
    }
}
