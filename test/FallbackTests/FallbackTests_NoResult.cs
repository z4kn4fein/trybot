using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Fallback;

namespace Trybot.Tests.FallbackTests
{
    [TestClass]
    public class FallbackTestsNoResult
    {
        private IBotPolicy CreatePolicy(FallbackConfiguration conf) =>
            new BotPolicy(config => config
                .Configure(botconfig => botconfig
                    .Fallback(conf)));

        private FallbackConfiguration CreateConfiguration() =>
            new FallbackConfiguration()
                .WhenExceptionOccurs(ex => true);

        [TestMethod]
        public void FallbackTests_Ok()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration());
            var counter = 0;
            policy.Execute((ex, t) => counter++, CancellationToken.None);

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public async Task FallbackTests_Async_Ok()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration());
            var counter = 0;
            await policy.ExecuteAsync((ex, t) => counter++, CancellationToken.None);

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public async Task FallbackTests_Async_Task_Ok()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration());
            var counter = 0;
            await policy.ExecuteAsync((ex, t) => { counter++; return Task.FromResult(0); }, CancellationToken.None);

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public void FallbackTests_Fail()
        {
            var counter = 0;
            var policy = this.CreatePolicy(this.CreateConfiguration()
                .OnFallback((ex, ctx) => counter++));
            policy.Execute((ex, t) => throw new Exception(), CancellationToken.None);

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public void FallbackTests_Fail_Handles_Only_ConfiguredException()
        {
            var counter = 0;
            var policy = this.CreatePolicy(this.CreateConfiguration()
                .WhenExceptionOccurs(ex => ex is NullReferenceException)
                .OnFallback((ex, ctx) => counter++));
            Assert.ThrowsException<InvalidOperationException>(() => policy.Execute((ex, t) => throw new InvalidOperationException(), CancellationToken.None));

            Assert.AreEqual(0, counter);
        }

        [TestMethod]
        public async Task FallbackTests_Fail_Handles_Only_ConfiguredException_Async()
        {
            var counter = 0;
            var policy = this.CreatePolicy(this.CreateConfiguration()
                .WhenExceptionOccurs(ex => ex is NullReferenceException)
                .OnFallback((ex, ctx) => counter++));
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => policy.ExecuteAsync((ex, t) => throw new InvalidOperationException(), CancellationToken.None));

            Assert.AreEqual(0, counter);
        }

        [TestMethod]
        public async Task FallbackTests_Async_Fail()
        {
            var counter = 0;
            var policy = this.CreatePolicy(this.CreateConfiguration()
                .OnFallback((ex, ctx) => counter++)
                .OnFallbackAsync((ex, ctx, t) => { counter++; return Task.FromResult(0); }));
            await policy.ExecuteAsync((ex, t) => throw new Exception(), CancellationToken.None);

            Assert.AreEqual(2, counter);
        }
    }
}
