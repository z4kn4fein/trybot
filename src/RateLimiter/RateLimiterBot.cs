using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;
using Trybot.RateLimiter.Exceptions;
using Trybot.Utils;

namespace Trybot.RateLimiter
{
    internal class RateLimiterBot : ConfigurableBot<RateLimiterConfiguration>
    {
        private readonly RateLimiterStrategy strategy;

        public RateLimiterBot(Bot innerBot, RateLimiterConfiguration configuration) : base(innerBot, configuration)
        {
            this.strategy = configuration.StrategyFactory(configuration.MaxOperationCount, configuration.Interval);
        }

        public override void Execute(IBotOperation operation, ExecutionContext context, CancellationToken token)
        {
            if (this.strategy.ShouldLimit(out var retryAfter))
                throw new RateLimitExceededException(Constants.RateLimitExceededExceptionMessage, retryAfter);

            base.InnerBot.Execute(operation, context, token);
        }

        public override Task ExecuteAsync(IAsyncBotOperation operation, ExecutionContext context, CancellationToken token)
        {
            if (this.strategy.ShouldLimit(out var retryAfter))
                throw new RateLimitExceededException(Constants.RateLimitExceededExceptionMessage, retryAfter);

            return base.InnerBot.ExecuteAsync(operation, context, token);
        }
    }
}
