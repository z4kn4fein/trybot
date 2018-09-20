using System;
using System.Collections.Generic;

namespace Trybot
{
    /// <summary>
    /// Represents the contextual data related to an actual execution.
    /// </summary>
    public class ExecutionContext
    {
        internal static ExecutionContext New(BotPolicyConfiguration configuration) =>
            new ExecutionContext(configuration);

        internal static ExecutionContext New(BotPolicyConfiguration configuration, Guid correlationId) =>
            new ExecutionContext(configuration, correlationId);

        /// <summary>
        /// Configuration of the bot policy.
        /// </summary>
        public BotPolicyConfiguration BotPolicyConfiguration { get; }

        /// <summary>
        /// The correlation id.
        /// </summary>
        public Guid CorrelationId { get; }

        /// <summary>
        /// Generic data store.
        /// </summary>
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
