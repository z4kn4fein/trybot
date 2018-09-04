using System;
using System.Threading.Tasks;
using System.Threading;

namespace Trybot.Timeout
{
    public class TimeoutBot : ConfigurableBot<TimeoutConfiguration>
    {
        public TimeoutBot(Bot innerPolicy, TimeoutConfiguration configuration) : base(innerPolicy, configuration)
        { }

        public override async Task ExecuteAsync(Action<ExecutionContext, CancellationToken> operation,
            ExecutionContext context, CancellationToken token) 
        {

        }

        public override async Task ExecuteAsync(Func<ExecutionContext, CancellationToken, Task> operation, ExecutionContext context, CancellationToken token)
        {

        }

        public override void Execute(Action<ExecutionContext, CancellationToken> operation,
            ExecutionContext context, CancellationToken token)
        {

        }
    }
}