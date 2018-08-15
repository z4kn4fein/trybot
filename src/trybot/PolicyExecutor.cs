using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot
{
    public class PolicyExecutor : IPolicyExecutor, IExecutorConfigurator
    {
        private readonly ExecutorConfiguration configuration;

        private AvlTreeKeyValue<object, Bot> policies;

        public PolicyExecutor(Action<IExecutorConfigurator> configuratorAction = null)
        {
            this.configuration = new ExecutorConfiguration();
            this.policies = AvlTreeKeyValue<object, Bot>.Empty;

            configuratorAction?.Invoke(this);
        }

        public IExecutorConfigurator SetCapturedContextContinuation(bool continueOnCapturedContext = false)
        {
            this.configuration.ContinueOnCapturedContext = continueOnCapturedContext;
            return this;
        }

        public IExecutorConfigurator DefinePolicy(object identifier, Action<IPolicyConfigurator> configuratorAction)
        {
            var configurator = new PolicyConfigurator();
            configuratorAction(configurator);

            Swap.SwapValue(ref this.policies, p => p.AddOrUpdate(identifier, configurator.Bot));

            return this;
        }

        public void Execute(object policyIdentifier, Action<ExecutionContext, CancellationToken> action, CancellationToken token) =>
            this.GetPolicyById(policyIdentifier).Execute(action, ExecutionContext.New(this.configuration), token);

        public TResult Execute<TResult>(object policyIdentifier, Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token) =>
            this.GetPolicyById(policyIdentifier).Execute(operation, ExecutionContext.New(this.configuration), token);

        public async Task ExecuteAsync(object policyIdentifier, Action<ExecutionContext, CancellationToken> action, CancellationToken token) =>
            await this.GetPolicyById(policyIdentifier).ExecuteAsync(action, ExecutionContext.New(this.configuration), token)
                .ConfigureAwait(this.configuration.ContinueOnCapturedContext);

        public async Task<TResult> ExecuteAsync<TResult>(object policyIdentifier, Func<ExecutionContext, CancellationToken, TResult> operation, CancellationToken token) =>
            await this.GetPolicyById(policyIdentifier).ExecuteAsync(operation, ExecutionContext.New(this.configuration), token)
                .ConfigureAwait(this.configuration.ContinueOnCapturedContext);

        public async Task<TResult> ExecuteAsync<TResult>(object policyIdentifier, Func<ExecutionContext, CancellationToken, Task<TResult>> operation, CancellationToken token) =>
            await this.GetPolicyById(policyIdentifier).ExecuteAsync(operation, ExecutionContext.New(this.configuration), token)
                .ConfigureAwait(this.configuration.ContinueOnCapturedContext);

        private Bot GetPolicyById(object policyIdentifier)
        {
            var policy = this.policies.GetOrDefault(policyIdentifier);
            if (policy == null)
                throw new PolicyNotFoundException($"The policy with the given id ({policyIdentifier}) not found.", policyIdentifier);

            return policy;
        }
    }
}
