using System;

namespace Trybot.Retry.Model
{
    internal class TryResult
    {
        public bool IsSucceeded { get; set; }

        public Exception Exception { get; set; }

        public static TryResult Failed(Exception exception = null) => new TryResult { IsSucceeded = false, Exception = exception };

        public static TryResult Succeeded() => new TryResult { IsSucceeded = true };


        public static TryResult Default = new TryResult();
    }

    internal class TryResult<TResult> : TryResult
    {
        public TResult OperationResult { get; set; }

        public static TryResult<TResult> Failed(Exception exception = null, TResult result = default) => new TryResult<TResult> { IsSucceeded = false, Exception = exception, OperationResult = result };

        public static TryResult<TResult> Succeeded(TResult result = default) => new TryResult<TResult> { IsSucceeded = true, OperationResult = result };


        public static new TryResult<TResult> Default = new TryResult<TResult>();
    }
}
