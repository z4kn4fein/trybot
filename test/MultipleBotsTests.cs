using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

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

            policy.Execute((ctx, t) => { }, source.Token);

            mockBot.Verify(m => m.Execute(It.IsAny<IBotOperation>(), It.IsAny<ExecutionContext>(), source.Token));
        }

        [TestMethod]
        public void BotPolicyTest_Execute_Func()
        {
            var policy = new BotPolicy<int>();
            var mockBot = this.CreateMockBot(policy);

            var source = new CancellationTokenSource();

            policy.Execute((ctx, t) => 0, source.Token);

            mockBot.Verify(m => m.Execute(It.IsAny<IBotOperation<int>>(), It.IsAny<ExecutionContext>(), source.Token));
        }

        [TestMethod]
        public async Task BotPolicyTest_ExecuteAsync_Action()
        {
            var policy = new BotPolicy();
            var mockBot = this.CreateMockBot(policy);

            var source = new CancellationTokenSource();

            await policy.ExecuteAsync((ctx, t) => { }, source.Token);

            mockBot.Verify(m => m.ExecuteAsync(It.IsAny<IAsyncBotOperation>(), It.IsAny<ExecutionContext>(), source.Token));
        }

        [TestMethod]
        public async Task BotPolicyTest_ExecuteAsync_Func()
        {
            var policy = new BotPolicy();
            var mockBot = this.CreateMockBot(policy);

            var source = new CancellationTokenSource();

            await policy.ExecuteAsync((ctx, t) => Task.FromResult(0), source.Token);

            mockBot.Verify(m => m.ExecuteAsync(It.IsAny<IAsyncBotOperation>(), It.IsAny<ExecutionContext>(), source.Token));
        }

        [TestMethod]
        public async Task BotPolicyTest_ExecuteAsync_Func_Result()
        {
            var policy = new BotPolicy<int>();
            var mockBot = this.CreateMockBot(policy);

            var source = new CancellationTokenSource();

            await policy.ExecuteAsync((ctx, t) => 0, source.Token);

            mockBot.Verify(m => m.ExecuteAsync(It.IsAny<IAsyncBotOperation<int>>(), It.IsAny<ExecutionContext>(), source.Token));
        }

        [TestMethod]
        public async Task BotPolicyTest_ExecuteAsync_Func_Task_Result()
        {
            var policy = new BotPolicy<int>();
            var mockBot = this.CreateMockBot(policy);

            var source = new CancellationTokenSource();

            await policy.ExecuteAsync((ctx, t) => Task.FromResult(0), source.Token);

            mockBot.Verify(m => m.ExecuteAsync(It.IsAny<IAsyncBotOperation<int>>(), It.IsAny<ExecutionContext>(), source.Token));
        }
    }
}
