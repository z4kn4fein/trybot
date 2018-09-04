using System;

namespace Trybot.Model
{
    internal class TryResult
    {
        public static TryResult Default = new TryResult();

        public bool Succeeded { get; set; }
        public bool ForceThrowException { get; set; }
        public Exception Exception { get; set; }
        public object FunctionResult { get; set; }
    }
}
