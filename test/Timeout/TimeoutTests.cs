using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Timeout;
using Trybot.Timeout.Exceptions;

namespace Trybot.Tests.Timeout
{
    [TestClass]
    public class TimeoutTestsAsync
    {
        private IBotPolicy<T> CreatePolicyWithTimeout<T>(TimeoutConfiguration timeoutConfig) =>
            new BotPolicy<T>(config => config
                .Configure(botconfig => botconfig
                    .Timeout(timeoutConfig)));

        private TimeoutConfiguration CreateConfiguration(TimeSpan after) =>
            new TimeoutConfiguration()
                .After(after);

        [TestMethod]
        public void TimeoutTest_Ok_Result()
        {
            var policy = this.CreatePolicyWithTimeout<int>(this.CreateConfiguration(TimeSpan.FromSeconds(.2)));
            var result = policy
                .Execute((ex, t) => 5, CancellationToken.None);

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void TimeoutTest_Timeout_Result()
        {
            var policy = this.CreatePolicyWithTimeout<int>(this.CreateConfiguration(TimeSpan.FromSeconds(.2)));
            var result = 0;
            CancellationToken token;
            Assert.ThrowsException<OperationTimeoutException>(() => result = policy
                .Execute((ex, t) => { token = t; Task.Delay(TimeSpan.FromSeconds(5), t).Wait(t); return 5; }, CancellationToken.None));

            Assert.IsTrue(token.IsCancellationRequested);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public async Task TimeoutTest_Timeout_Result_Async()
        {
            var policy = this.CreatePolicyWithTimeout<int>(this.CreateConfiguration(TimeSpan.FromSeconds(.2)));
            var result = 0;
            CancellationToken token;
            await Assert.ThrowsExceptionAsync<OperationTimeoutException>(async () => result = await policy
                .ExecuteAsync((ex, t) => { token = t; Task.Delay(TimeSpan.FromSeconds(5), t).Wait(t); return Task.FromResult(5); }, CancellationToken.None));

            Assert.IsTrue(token.IsCancellationRequested);
            Assert.AreEqual(0, result);
        }
    }
}
