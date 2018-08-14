using System;

namespace Trybot.Retry
{
    internal class TryResult
    {
        public bool IsSucceeded { get; set; }
        public Exception Exception { get; set; }
        public object OperationResult { get; set; }

        public static TryResult Failed(Exception exception = null, object result = null) => new TryResult { IsSucceeded = false, Exception = exception, OperationResult = result };

        public static TryResult Succeeded(object result = null) => new TryResult { IsSucceeded = true, OperationResult = result };

        public static TryResult Default = new TryResult();
    }
}
