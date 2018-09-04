using System;
using System.Collections.Generic;

namespace Trybot
{
    public class ExecutionContext
    {
        internal static ExecutionContext New(BotPolicyConfiguration configuration) =>
            new ExecutionContext(configuration);

        internal static ExecutionContext New(BotPolicyConfiguration configuration, Guid correlationId) =>
            new ExecutionContext(configuration, correlationId);

        public BotPolicyConfiguration BotPolicyConfiguration { get; }

        public Guid CorrelationId { get; }

        public IDictionary<object, object> GenericData { get; }

        internal ExecutionContext(BotPolicyConfiguration configuration)
            : this(configuration, Guid.NewGuid())
        { }

        internal ExecutionContext(BotPolicyConfiguration configuration, Guid correlationId)
        {
            this.BotPolicyConfiguration = configuration;
            this.CorrelationId = correlationId;
            this.GenericData = new Dictionary<object, object>();
        }
    }
}
