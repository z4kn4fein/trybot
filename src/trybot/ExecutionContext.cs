using System;
using System.Collections.Generic;

namespace Trybot
{
    public class ExecutionContext
    {
        internal static ExecutionContext New(ExecutorConfiguration configuration) =>
            new ExecutionContext(configuration);

        public ExecutorConfiguration ExecutorConfiguration { get; }

        public Guid CorrelationId { get; }

        public IDictionary<object, object> GenericData { get; }

        internal ExecutionContext(ExecutorConfiguration configuration)
        {
            this.ExecutorConfiguration = configuration;
            this.CorrelationId = Guid.NewGuid();
            this.GenericData = new Dictionary<object, object>();
        }
    }
}
