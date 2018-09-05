using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Tests
{
    public class MockConfig { }

    [TestClass]
    public class MultipleBotsTests
    {
        private Mock<ConfigurableBot<MockConfig>> CreateMockBot(IBotPolicyConfigurator policy)
        {
            Mock<ConfigurableBot<MockConfig>> mockConfigBot = null;
            policy.Configure(config => config
                .AddBot<ConfigurableBot<MockConfig>, MockConfig>((bot, c) =>
                {
                    mockConfigBot = new Mock<ConfigurableBot<MockConfig>>(bot, c);
                    return mockConfigBot.Object;
                }, c => { })
                .Passthrough());

            return mockConfigBot;
        }

        private Mock<ConfigurableBot<MockConfig, int>> CreateMockBot(IBotPolicyConfigurator<int> policy)
        {
            Mock<ConfigurableBot<MockConfig, int>> mockConfigBot = null;
            policy.Configure(config => config
                .AddBot<ConfigurableBot<MockConfig, int>, MockConfig>((bot, c) =>
                {
                    mockConfigBot = new Mock<ConfigurableBot<MockConfig, int>>(bot, c);
                    return mockConfigBot.Object;
                }, c => { })
                .Passthrough());

            return mockConfigBot;
        }

        [TestMethod]
        public void BotPolicyTest_Execute_Action()
        {
            var policy = new BotPolicy();
            var mockBot = this.CreateMockBot(policy);

            var source = new CancellationTokenSource();
            Action<ExecutionContext, CancellationToken> action = (ctx, t) => { };

            policy.Execute(action, source.Token);

            mockBot.Verify(m => m.Execute(action, It.IsAny<ExecutionContext>(), source.Token));
        }

        [TestMethod]
        public void BotPolicyTest_Execute_Func()
        {
            var policy = new BotPolicy<int>();
            var mockBot = this.CreateMockBot(policy);

            var source = new CancellationTokenSource();
            Func<ExecutionContext, CancellationToken, int> operation = (ctx, t) => 0;

            policy.Execute(operation, source.Token);

            mockBot.Verify(m => m.Execute(operation, It.IsAny<ExecutionContext>(), source.Token));
        }

        [TestMethod]
        public async Task BotPolicyTest_ExecuteAsync_Action()
        {
            var policy = new BotPolicy();
            var mockBot = this.CreateMockBot(policy);

            var source = new CancellationTokenSource();
            Action<ExecutionContext, CancellationToken> action = (ctx, t) => { };

            await policy.ExecuteAsync(action, source.Token);

            mockBot.Verify(m => m.ExecuteAsync(action, It.IsAny<ExecutionContext>(), source.Token));
        }

        [TestMethod]
        public async Task BotPolicyTest_ExecuteAsync_Func()
        {
            var policy = new BotPolicy();
            var mockBot = this.CreateMockBot(policy);

            var source = new CancellationTokenSource();
            Func<ExecutionContext, CancellationToken, Task> operation = (ctx, t) => Task.FromResult(0);

            await policy.ExecuteAsync(operation, source.Token);

            mockBot.Verify(m => m.ExecuteAsync(operation, It.IsAny<ExecutionContext>(), source.Token));
        }

        [TestMethod]
        public async Task BotPolicyTest_ExecuteAsync_Func_Result()
        {
            var policy = new BotPolicy<int>();
            var mockBot = this.CreateMockBot(policy);

            var source = new CancellationTokenSource();
            Func<ExecutionContext, CancellationToken, int> operation = (ctx, t) => 0;

            await policy.ExecuteAsync(operation, source.Token);

            mockBot.Verify(m => m.ExecuteAsync(operation, It.IsAny<ExecutionContext>(), source.Token));
        }

        [TestMethod]
        public async Task BotPolicyTest_ExecuteAsync_Func_Task_Result()
        {
            var policy = new BotPolicy<int>();
            var mockBot = this.CreateMockBot(policy);

            var source = new CancellationTokenSource();
            Func<ExecutionContext, CancellationToken, Task<int>> operation = (ctx, t) => Task.FromResult(0);

            await policy.ExecuteAsync(operation, source.Token);

            mockBot.Verify(m => m.ExecuteAsync(operation, It.IsAny<ExecutionContext>(), source.Token));
        }
    }
}
