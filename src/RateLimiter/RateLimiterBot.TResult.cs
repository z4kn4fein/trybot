using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;
using Trybot.RateLimiter.Exceptions;
using Trybot.Utils;

namespace Trybot.RateLimiter
{
    internal class RateLimiterBot<TResult> : ConfigurableBot<RateLimiterConfiguration, TResult>
    {
        private readonly RateLimiterStrategy strategy;

        public RateLimiterBot(Bot<TResult> innerBot, RateLimiterConfiguration configuration) : base(innerBot, configuration)
        {
            this.strategy = configuration.StrategyFactory(configuration.MaxOperationCount, configuration.Interval);
        }

        public override TResult Execute(IBotOperation<TResult> operation, ExecutionContext context, CancellationToken token)
        {
            if (this.strategy.ShouldLimit(out var retryAfter))
                throw new RateLimitExceededException(Constants.RateLimitExceededExceptionMessage, retryAfter);

            return base.InnerBot.Execute(operation, context, token);
        }

        public override Task<TResult> ExecuteAsync(IAsyncBotOperation<TResult> operation, ExecutionContext context, CancellationToken token)
        {
            if (this.strategy.ShouldLimit(out var retryAfter))
                throw new RateLimitExceededException(Constants.RateLimitExceededExceptionMessage, retryAfter);

            return base.InnerBot.ExecuteAsync(operation, context, token);
        }
    }
}
