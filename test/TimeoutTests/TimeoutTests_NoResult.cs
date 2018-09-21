using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Timeout;
using Trybot.Timeout.Exceptions;

namespace Trybot.Tests.TimeoutTests
{
    [TestClass]
    public class TimeoutTestsNoResult
    {
        private IBotPolicy CreatePolicyWithTimeout(TimeoutConfiguration timeoutConfig) =>
            new BotPolicy(config => config
                .Configure(botconfig => botconfig
                    .Timeout(timeoutConfig)));

        private TimeoutConfiguration CreateConfiguration(TimeSpan after) =>
            new TimeoutConfiguration()
                .After(after);

        [TestMethod]
        public void TimeoutTest_Ok()
        {
            var policy = this.CreatePolicyWithTimeout(this.CreateConfiguration(TimeSpan.FromSeconds(.2)));
            CancellationToken token;
            policy.Execute((ex, t) => { token = t; }, CancellationToken.None);

            Assert.IsFalse(token.IsCancellationRequested);
        }

        [TestMethod]
        public async Task TimeoutTest_Ok_Async()
        {
            var policy = this.CreatePolicyWithTimeout(this.CreateConfiguration(TimeSpan.FromSeconds(.2)));
            CancellationToken token;
            await policy.ExecuteAsync((ex, t) => { token = t; }, CancellationToken.None);

            Assert.IsFalse(token.IsCancellationRequested);
        }

        [TestMethod]
        public void TimeoutTest_Config()
        {
            var policy = new BotPolicy(config => config
                .Configure(botconfig => botconfig
                    .Timeout(timeoutConfig => timeoutConfig
                        .After(TimeSpan.FromSeconds(.2)))));

            Assert.ThrowsException<OperationTimeoutException>(() => policy.Execute((ctx, t) =>
                Task.Delay(500, t).Wait(t), CancellationToken.None));
        }

        [TestMethod]
        public void TimeoutTest_Timeout()
        {
            var policy = this.CreatePolicyWithTimeout(this.CreateConfiguration(TimeSpan.FromSeconds(.2)));
            CancellationToken token;
            Assert.ThrowsException<OperationTimeoutException>(() =>
                policy.Execute((ex, t) => { token = t; Task.Delay(TimeSpan.FromSeconds(5), t).Wait(t); }, CancellationToken.None));

            Assert.IsTrue(token.IsCancellationRequested);
        }

        [TestMethod]
        public void TimeoutTest_Timeout_OnTimeout()
        {
            var onTimeoutCalled = false;
            var policy = this.CreatePolicyWithTimeout(this.CreateConfiguration(TimeSpan.FromSeconds(.2))
                .OnTimeout(ex => onTimeoutCalled = true)
                .OnTimeoutAsync(ex => { Assert.Fail("OnTimeoutAsync called from sync caller!"); return Task.FromResult(0); }));
            CancellationToken token;
            Assert.ThrowsException<OperationTimeoutException>(() =>
                policy.Execute((ex, t) => { token = t; Task.Delay(TimeSpan.FromSeconds(5), t).Wait(t); }, CancellationToken.None));

            Assert.IsTrue(token.IsCancellationRequested);
            Assert.IsTrue(onTimeoutCalled);
        }

        [TestMethod]
        public async Task TimeoutTest_Timeout_OnTimeoutAsync()
        {
            var onTimeoutCalled = false;
            var onTimeoutCalledAsync = false;
            var policy = this.CreatePolicyWithTimeout(this.CreateConfiguration(TimeSpan.FromSeconds(.2))
                .OnTimeout(ex => onTimeoutCalled = true)
                .OnTimeoutAsync(ex => { onTimeoutCalledAsync = true; return Task.FromResult(0); }));
            CancellationToken token;
            await Assert.ThrowsExceptionAsync<OperationTimeoutException>(() =>
                policy.ExecuteAsync((ex, t) => { token = t; return Task.Delay(TimeSpan.FromSeconds(5), t); }, CancellationToken.None));

            Assert.IsTrue(token.IsCancellationRequested, "token.IsCancellationRequested");
            Assert.IsTrue(onTimeoutCalled, "onTimeoutCalled");
            Assert.IsTrue(onTimeoutCalledAsync, "onTimeoutCalledAsync");
        }

        [TestMethod]
        public async Task TimeoutTest_Timeout_Async()
        {
            var policy = this.CreatePolicyWithTimeout(this.CreateConfiguration(TimeSpan.FromSeconds(.2)));
            CancellationToken token;
            await Assert.ThrowsExceptionAsync<OperationTimeoutException>(() =>
                policy.ExecuteAsync((ex, t) => { token = t; Task.Delay(TimeSpan.FromSeconds(5), t).Wait(t); }, CancellationToken.None));

            Assert.IsTrue(token.IsCancellationRequested);
        }
    }
}
