using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;
using Trybot.Timeout.Exceptions;
using Trybot.Utils;

namespace Trybot.Timeout
{
    internal class TimeoutBot : ConfigurableBot<TimeoutConfiguration>
    {
        internal TimeoutBot(Bot innerPolicy, TimeoutConfiguration configuration) : base(innerPolicy, configuration)
        { }

        public override void Execute(IBotOperation operation,
            ExecutionContext context, CancellationToken token)
        {
            using (var timeoutTokenSource = new CancellationTokenSource())
            using (var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutTokenSource.Token))
            {
                try
                {
                    timeoutTokenSource.CancelAfter(base.Configuration.Timeout);
                    base.InnerBot.Execute(operation, context, combinedTokenSource.Token);
                }
                catch (Exception ex)
                {
                    if (!timeoutTokenSource.IsCancellationRequested) throw;

                    base.Configuration.RaiseTimeoutEvent(context);
                    throw new OperationTimeoutException(Constants.TimeoutExceptionMessage, ex);

                }
            }
        }

        public override async Task ExecuteAsync(IAsyncBotOperation operation,
            ExecutionContext context, CancellationToken token)
        {
            using (var timeoutTokenSource = new CancellationTokenSource())
            using (var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, timeoutTokenSource.Token))
            {
                try
                {
                    timeoutTokenSource.CancelAfter(base.Configuration.Timeout);
                    await base.InnerBot.ExecuteAsync(operation, context, combinedTokenSource.Token)
                        .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
                }
                catch (Exception ex)
                {
                    if (!timeoutTokenSource.IsCancellationRequested) throw;

                    await base.Configuration.RaiseAsyncTimeoutEvent(context)
                        .ConfigureAwait(context.BotPolicyConfiguration.ContinueOnCapturedContext);
                    throw new OperationTimeoutException(Constants.TimeoutExceptionMessage, ex);

                }
            }
        }
    }
}