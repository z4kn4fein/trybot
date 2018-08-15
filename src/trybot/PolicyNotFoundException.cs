using System;

namespace Trybot
{
    public class PolicyNotFoundException : Exception
    {
        public object PolicyIdentifier { get; }

        public PolicyNotFoundException(string message, object policyIdentifier) : base(message)
        {
            this.PolicyIdentifier = policyIdentifier;
        }
    }
}
