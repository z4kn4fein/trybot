using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using Trybot.Strategy;

namespace Trybot.Tests
{
    [TestClass]
    public class RetryStrategyTests
    {
        [TestMethod]
        public void RetryStrategyTests_DefaultRetryStrategy_Set()
        {
            var strategy = new FixedIntervalRetryStartegy(5, TimeSpan.MaxValue);
            RetryStartegy.DefaultRetryStrategy = strategy;
            Assert.AreEqual(strategy, RetryStartegy.DefaultRetryStrategy);
        }

        [TestMethod]
        public void RetryStrategyTests_CubicRetryStrategyTest()
        {
            var strategy = new CubicRetryStrategy(5, TimeSpan.FromMilliseconds(5));
            var privateObject = new PrivateObject(strategy);
            var result = (TimeSpan)privateObject.Invoke("GetNextDelay", BindingFlags.Instance | BindingFlags.NonPublic, 2);
            Assert.AreEqual(TimeSpan.FromMilliseconds(1000), result);
        }

        [TestMethod]
        public void RetryStrategyTests_LinearRetryStrategyTest()
        {
            var strategy = new LinearRetryStrategy(5, TimeSpan.FromMilliseconds(5));
            var privateObject = new PrivateObject(strategy);
            var result = (TimeSpan)privateObject.Invoke("GetNextDelay", BindingFlags.Instance | BindingFlags.NonPublic, 2);
            Assert.AreEqual(TimeSpan.FromMilliseconds(10), result);
        }

        [TestMethod]
        public void RetryStrategyTests_SquareRetryStrategyTest()
        {
            var strategy = new SquareRetryStartegy(5, TimeSpan.FromMilliseconds(5));
            var privateObject = new PrivateObject(strategy);
            var result = (TimeSpan)privateObject.Invoke("GetNextDelay", BindingFlags.Instance | BindingFlags.NonPublic, 2);
            Assert.AreEqual(TimeSpan.FromMilliseconds(100), result);
        }
    }
}
