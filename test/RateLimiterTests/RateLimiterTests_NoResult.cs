using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using Trybot.RateLimiter;
using Trybot.RateLimiter.Exceptions;

namespace Trybot.Tests.RateLimiterTests
{
    [TestClass]
    public class RateLimiterTestsNoResult
    {
        private IBotPolicy CreatePolicyWithRateLimit(RateLimiterConfiguration configuration) =>
            new BotPolicy(config => config
                .Configure(botconfig => botconfig
                    .RateLimit(configuration)));

        private RateLimiterConfiguration CreateConfiguration(int count, TimeSpan interval) =>
            new RateLimiterConfiguration()
                .MaxAmountOfAllowedOperations(count)
                .WithinTimeInterval(interval);

        [TestMethod]
        public void RateLimit_Sliding_Ok()
        {
            var called = false;
            var policy = this.CreatePolicyWithRateLimit(this.CreateConfiguration(5, TimeSpan.FromSeconds(10)));
            policy.Execute(() => { called = true; });
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void RateLimit_Sliding_Reject()
        {
            var policy = this.CreatePolicyWithRateLimit(this.CreateConfiguration(2, TimeSpan.FromSeconds(2)));

            policy.Execute(() => { });
            policy.Execute(() => { });
            Assert.ThrowsException<RateLimitExceededException>(() => policy.Execute(() => { }));
        }

        [TestMethod]
        public void RateLimit_Sliding_Reject_Allow_Again()
        {
            var policy = this.CreatePolicyWithRateLimit(this.CreateConfiguration(2, TimeSpan.FromSeconds(2)));

            policy.Execute(() => { });
            Thread.Sleep(1000);
            policy.Execute(() => { });
            Assert.ThrowsException<RateLimitExceededException>(() => policy.Execute(() => { }));
            Thread.Sleep(1000);
            policy.Execute(() => { });
        }
    }
}
