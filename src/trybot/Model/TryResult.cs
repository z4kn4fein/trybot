using System;

namespace Trybot.Model
{
    public class TryResult
    {
        public bool Succeeded { get; set; }
        public bool ForceThrowException { get; set; }
        public Exception Exception { get; set; }
        public object FunctionResult { get; set; }
    }
}
