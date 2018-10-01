using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Operations;

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

            policy.Execute((ctx, t) => { }, source.Token);

            mockBot.Verify(m => m.Execute(It.IsAny<IBotOperation>(), It.IsAny<ExecutionContext>(), source.Token));
        }

        [TestMethod]
        public void BotPolicyTest_Execute_Action_Without_Parameters()
        {
            var policy = new BotPolicy();
            var mockBot = this.CreateMockBot(policy);

            policy.Execute(() => { });

            mockBot.Verify(m => m.Execute(It.IsAny<IBotOperation>(), It.IsAny<ExecutionContext>(), CancellationToken.None));
        }

        [TestMethod]
        public void BotPolicyTest_Execute_Action_Without_Parameters_CorrelationId()
        {
            var policy = new BotPolicy();
            var mockBot = this.CreateMockBot(policy);
            var correlationId = new object();

            mockBot.Setup(m => m.Execute(It.IsAny<IBotOperation>(), It.IsAny<ExecutionContext>(), CancellationToken.None))
                .Callback<IBotOperation, ExecutionContext, CancellationToken>((o, ctx, t) => Assert.AreEqual(correlationId, ctx.CorrelationId))
                .Verifiable();

            policy.Execute(() => { }, correlationId);
        }

        [TestMethod]
        public void BotPolicyTest_Config()
        {
            var policy = new BotPolicy();
            policy.SetCapturedContextContinuation(true);
            policy.Execute((ctx, t) => Assert.IsTrue(ctx.BotPolicyConfiguration.ContinueOnCapturedContext), CancellationToken.None);
        }

        [TestMethod]
        public void BotPolicyTest_Config_Result()
        {
            var policy = new BotPolicy<int>();
            policy.SetCapturedContextContinuation(true);
            policy.Execute((ctx, t) =>
            {
                Assert.IsTrue(ctx.BotPolicyConfiguration.ContinueOnCapturedContext);
                return 0;
            }, CancellationToken.None);
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
        public void BotPolicyTest_Execute_Func_Without_Parameters()
        {
            var policy = new BotPolicy<int>();
            var mockBot = this.CreateMockBot(policy);

            policy.Execute(() => 0);

            mockBot.Verify(m => m.Execute(It.IsAny<IBotOperation<int>>(), It.IsAny<ExecutionContext>(), CancellationToken.None));
        }

        [TestMethod]
        public void BotPolicyTest_Execute_Func_Without_Parameters_With_CorrelationId()
        {
            var policy = new BotPolicy<int>();
            var mockBot = this.CreateMockBot(policy);
            var correlationId = new object();

            mockBot.Setup(m => m.Execute(It.IsAny<IBotOperation<int>>(), It.IsAny<ExecutionContext>(), CancellationToken.None))
                .Callback<IBotOperation<int>, ExecutionContext, CancellationToken>((o, ctx, t) => Assert.AreEqual(correlationId, ctx.CorrelationId))
                .Returns(0)
                .Verifiable();

            policy.Execute(() => 0, correlationId);
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
        public async Task BotPolicyTest_ExecuteAsync_Action_Without_Parameters()
        {
            var policy = new BotPolicy();
            var mockBot = this.CreateMockBot(policy);

            await policy.ExecuteAsync(() => { });

            mockBot.Verify(m => m.ExecuteAsync(It.IsAny<IAsyncBotOperation>(), It.IsAny<ExecutionContext>(), CancellationToken.None));
        }

        [TestMethod]
        public async Task BotPolicyTest_ExecuteAsync_Action_Without_Parameters_With_CorrelationId()
        {
            var policy = new BotPolicy();
            var mockBot = this.CreateMockBot(policy);
            var correlationId = new object();

            mockBot.Setup(m => m.ExecuteAsync(It.IsAny<IAsyncBotOperation>(), It.IsAny<ExecutionContext>(), CancellationToken.None))
                .Callback<IAsyncBotOperation, ExecutionContext, CancellationToken>((o, ctx, t) => Assert.AreEqual(correlationId, ctx.CorrelationId))
                .Returns(Task.FromResult(0))
                .Verifiable();

            await policy.ExecuteAsync(() => { }, correlationId);
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
        public async Task BotPolicyTest_ExecuteAsync_Func_Without_Parameters()
        {
            var policy = new BotPolicy();
            var mockBot = this.CreateMockBot(policy);

            await policy.ExecuteAsync(() => Task.FromResult(0));

            mockBot.Verify(m => m.ExecuteAsync(It.IsAny<IAsyncBotOperation>(), It.IsAny<ExecutionContext>(), CancellationToken.None));
        }

        [TestMethod]
        public async Task BotPolicyTest_ExecuteAsync_Func_Without_Parameters_With_CorrelationId()
        {
            var policy = new BotPolicy();
            var mockBot = this.CreateMockBot(policy);
            var correlationId = new object();

            mockBot.Setup(m => m.ExecuteAsync(It.IsAny<IAsyncBotOperation>(), It.IsAny<ExecutionContext>(), CancellationToken.None))
                .Callback<IAsyncBotOperation, ExecutionContext, CancellationToken>((o, ctx, t) => Assert.AreEqual(correlationId, ctx.CorrelationId))
                .Returns(Task.FromResult(0))
                .Verifiable();

            await policy.ExecuteAsync(() => Task.FromResult(0), correlationId);
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
        public async Task BotPolicyTest_ExecuteAsync_Func_Result_Without_Parameters()
        {
            var policy = new BotPolicy<int>();
            var mockBot = this.CreateMockBot(policy);

            await policy.ExecuteAsync(() => 0);

            mockBot.Verify(m => m.ExecuteAsync(It.IsAny<IAsyncBotOperation<int>>(), It.IsAny<ExecutionContext>(), CancellationToken.None));
        }

        [TestMethod]
        public async Task BotPolicyTest_ExecuteAsync_Func_Result_Without_Parameters_With_CorrelationId()
        {
            var policy = new BotPolicy<int>();
            var mockBot = this.CreateMockBot(policy);
            var correlationId = new object();

            mockBot.Setup(m => m.ExecuteAsync(It.IsAny<IAsyncBotOperation<int>>(), It.IsAny<ExecutionContext>(), CancellationToken.None))
                .Callback<IAsyncBotOperation<int>, ExecutionContext, CancellationToken>((o, ctx, t) => Assert.AreEqual(correlationId, ctx.CorrelationId))
                .Returns(Task.FromResult(0))
                .Verifiable();

            await policy.ExecuteAsync(() => 0, correlationId);
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

        [TestMethod]
        public async Task BotPolicyTest_ExecuteAsync_Func_Task_Result_Without_Parameters()
        {
            var policy = new BotPolicy<int>();
            var mockBot = this.CreateMockBot(policy);

            await policy.ExecuteAsync(() => Task.FromResult(0));

            mockBot.Verify(m => m.ExecuteAsync(It.IsAny<IAsyncBotOperation<int>>(), It.IsAny<ExecutionContext>(), CancellationToken.None));
        }

        [TestMethod]
        public async Task BotPolicyTest_ExecuteAsync_Func_Task_Result_Without_Parameters_With_CorrelationId()
        {
            var policy = new BotPolicy<int>();
            var mockBot = this.CreateMockBot(policy);
            var correlationId = new object();

            mockBot.Setup(m => m.ExecuteAsync(It.IsAny<IAsyncBotOperation<int>>(), It.IsAny<ExecutionContext>(), CancellationToken.None))
                .Callback<IAsyncBotOperation<int>, ExecutionContext, CancellationToken>((o, ctx, t) => Assert.AreEqual(correlationId, ctx.CorrelationId))
                .Returns(Task.FromResult(0))
                .Verifiable();

            await policy.ExecuteAsync(() => Task.FromResult(0), correlationId);
        }
    }
}
