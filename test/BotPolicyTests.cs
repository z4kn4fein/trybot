using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Tests
{
    [TestClass]
    public class BotPolicyTests
    {
        private Mock<Bot> CreateMockBot(IBotPolicyConfigurator policy)
        {
            Mock<Bot> mockBot = null;
            policy.Configure(config => config.AddBot(bot =>
            {
                mockBot = new Mock<Bot>(bot);
                return mockBot.Object;
            }));

            return mockBot;
        }

        private Mock<Bot<int>> CreateMockBot(IBotPolicyConfigurator<int> policy)
        {
            Mock<Bot<int>> mockBot = null;
            policy.Configure(config => config.AddBot(bot =>
            {
                mockBot = new Mock<Bot<int>>(bot);
                return mockBot.Object;
            }));

            return mockBot;
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
