using System.Collections.Generic;

namespace Trybot
{
    /// <summary>
    /// Represents the contextual data related to an actual execution.
    /// </summary>
    public class ExecutionContext
    {
        internal static ExecutionContext New(BotPolicyConfiguration configuration, object correlationId) =>
            new ExecutionContext(configuration, correlationId);

        /// <summary>
        /// Configuration of the bot policy.
        /// </summary>
        public BotPolicyConfiguration BotPolicyConfiguration { get; }

        /// <summary>
        /// A unique id for each execution.
        /// </summary>
        public object CorrelationId { get; }

        /// <summary>
        /// Generic data store.
        /// </summary>
        public IDictionary<object, object> GenericData { get; }

        internal ExecutionContext(BotPolicyConfiguration configuration, object correlationId)
        {
            this.BotPolicyConfiguration = configuration;
            this.CorrelationId = correlationId;
            this.GenericData = new Dictionary<object, object>();
        }
    }
}
