using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Fallback;

namespace Trybot.Tests.FallbackTests
{
    [TestClass]
    public class FallbackTests
    {
        private IBotPolicy<T> CreatePolicy<T>(FallbackConfiguration<T> conf) =>
            new BotPolicy<T>(config => config
                .Configure(botconfig => botconfig
                    .Fallback(conf)));

        private FallbackConfiguration<T> CreateConfiguration<T>() =>
            new FallbackConfiguration<T>()
                .WhenExceptionOccurs(ex => true);

        [TestMethod]
        public void FallbackTests_Ok()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration<int>());
            var result = policy.Execute((ex, t) => 5, CancellationToken.None);

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void FallbackTests_Ok_Config()
        {
            var policy = new BotPolicy<int>(config => config
                .Configure(botconfig => botconfig
                    .Fallback(conf => conf.WhenExceptionOccurs(ex => true))));
            var result = policy.Execute((ex, t) => 5, CancellationToken.None);

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void FallbackTests_Handles_Only_Configured_Exception()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration<int>()
                .WhenExceptionOccurs(ex => ex is NullReferenceException));
            Assert.ThrowsException<InvalidOperationException>(() =>
                policy.Execute((ex, t) => throw new InvalidOperationException(), CancellationToken.None));
        }

        [TestMethod]
        public async Task FallbackTests_Handles_Only_Configured_Exception_Async()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration<int>()
                .WhenExceptionOccurs(ex => ex is InvalidOperationException));
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() =>
                policy.ExecuteAsync((ex, t) =>
                {
                    object o = null;
                    o.GetHashCode();
                    return 0;
                }, CancellationToken.None));
        }

        [TestMethod]
        public async Task FallbackTests_Async_Ok()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration<int>());
            var result = await policy.ExecuteAsync((ex, t) => 5, CancellationToken.None);

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public async Task FallbackTests_Async_Task_Ok()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration<int>());
            var result = await policy.ExecuteAsync((ex, t) => Task.FromResult(5), CancellationToken.None);

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void FallbackTests_Fail()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration<int>()
                .OnFallback((r, ex, ctx) => 6));
            var result = policy.Execute((ex, t) => throw new Exception(), CancellationToken.None);

            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void FallbackTests_Fail_ResultFilter()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration<int>()
                .WhenResultIs(r => r != 0)
                .OnFallback((r, ex, ctx) => 6));
            var result = policy.Execute((ex, t) => 5, CancellationToken.None);

            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public async Task FallbackTests_Fail_ResultFilter_Async()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration<int>()
                .WhenResultIs(r => r != 0)
                .OnFallback((r, ex, ctx) => 6));
            var result = await policy.ExecuteAsync((ex, t) => 5, CancellationToken.None);

            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public async Task FallbackTests_Fail_Async()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration<int>()
                .OnFallback((r, ex, ctx) => 6));
            var result = await policy.ExecuteAsync((ex, t) =>
            {
                object o = null;
                o.GetHashCode();
                return 5;
            }, CancellationToken.None);

            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public async Task FallbackTests_Fail_Async_OnFallback_Async()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration<int>()
                .OnFallbackAsync((r, ex, ctx, t) => Task.FromResult(6)));
            var result = await policy.ExecuteAsync((ex, t) =>
            {
                object o = null;
                o.GetHashCode();
                return Task.FromResult(5);
            }, CancellationToken.None);

            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public async Task FallbackTests_Fail_Async_OnFallback_Choose_Sync_When_Both_Set()
        {
            var policy = this.CreatePolicy(this.CreateConfiguration<int>()
                .OnFallback((r, ex, ctx) => 8)
                .OnFallbackAsync((r, ex, ctx, t) => Task.FromResult(6)));
            var result = await policy.ExecuteAsync((ex, t) =>
            {
                object o = null;
                o.GetHashCode();
                return Task.FromResult(5);
            }, CancellationToken.None);

            Assert.AreEqual(8, result);
        }
    }
}
