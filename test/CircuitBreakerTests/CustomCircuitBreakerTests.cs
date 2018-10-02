//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using System;
//using System.Threading;
//using Trybot.CircuitBreaker;
//using Trybot.CircuitBreaker.Exceptions;

//namespace Trybot.Tests.CircuitBreakerTests
//{
//    [TestClass]
//    public class CustomCircuitBreakerTests
//    {
//        [TestMethod]
//        public void CustomCircuitBreakerTests_Closed_Open_HalfOpen_Then_Closed()
//        {
//            var state = State.Closed;

//            Mock<CircuitBreakerStrategy> mockStrategy = null;
//            ICircuitBreakerStateSwitcher switcher = null;
//            var policy = new BotPolicy(config => config
//                .Configure(botConfig => botConfig
//                    .CustomCircuitBreaker(sw =>
//                    {
//                        switcher = sw;
//                        mockStrategy = new Mock<CircuitBreakerStrategy>(switcher);
//                        return mockStrategy.Object;
//                    }, cbConfig => cbConfig.BrakeWhenExceptionOccurs(ex => true)
//                        .OnClosed(() => state = State.Closed)
//                        .OnHalfOpen(() => state = State.HalfOpen)
//                        .OnOpen(ts => state = State.Open))));

//            mockStrategy.Setup(s => s.OperationFailedInClosed()).Callback(() => switcher.Open(TimeSpan.FromSeconds(.2))).Verifiable();

//            Assert.ThrowsException<InvalidOperationException>(() =>
//                policy.Execute((ctx, t) => throw new InvalidOperationException(), CancellationToken.None));

//            Assert.AreEqual(State.Open, state);

//            Thread.Sleep(TimeSpan.FromSeconds(.2));

//            mockStrategy.Setup(s => s.OperationSucceededInHalfOpen()).Callback(() => switcher.Close()).Verifiable();

//            policy.Execute((ctx, t) => Assert.AreEqual(State.HalfOpen, state), CancellationToken.None);

//            Assert.AreEqual(State.Closed, state);
//        }

//        [TestMethod]
//        public void CustomCircuitBreakerTests_Closed_Open_HalfOpen_Then_Closed_WithConfig()
//        {
//            var state = State.Closed;

//            Mock<CircuitBreakerStrategy> mockStrategy = null;
//            ICircuitBreakerStateSwitcher switcher = null;
//            var policy = new BotPolicy(config => config
//                .Configure(botConfig => botConfig
//                    .CustomCircuitBreaker(sw =>
//                    {
//                        switcher = sw;
//                        mockStrategy = new Mock<CircuitBreakerStrategy>(switcher);
//                        return mockStrategy.Object;
//                    }, new CircuitBreakerConfiguration().BrakeWhenExceptionOccurs(ex => true)
//                        .OnClosed(() => state = State.Closed)
//                        .OnHalfOpen(() => state = State.HalfOpen)
//                        .OnOpen(ts => state = State.Open))));

//            mockStrategy.Setup(s => s.OperationFailedInClosed()).Callback(() => switcher.Open(TimeSpan.FromSeconds(.2))).Verifiable();

//            Assert.ThrowsException<InvalidOperationException>(() =>
//                policy.Execute((ctx, t) => throw new InvalidOperationException(), CancellationToken.None));

//            Assert.AreEqual(State.Open, state);

//            Thread.Sleep(TimeSpan.FromSeconds(.2));

//            mockStrategy.Setup(s => s.OperationSucceededInHalfOpen()).Callback(() => switcher.Close()).Verifiable();

//            policy.Execute((ctx, t) => Assert.AreEqual(State.HalfOpen, state), CancellationToken.None);

//            Assert.AreEqual(State.Closed, state);
//        }

//        [TestMethod]
//        public void CustomCircuitBreakerTests_Closed_Open_HalfOpen_Then_Open()
//        {
//            var state = State.Closed;

//            Mock<CircuitBreakerStrategy> mockStrategy = null;
//            ICircuitBreakerStateSwitcher switcher = null;
//            var policy = new BotPolicy(config => config
//                .Configure(botConfig => botConfig
//                    .CustomCircuitBreaker(sw =>
//                    {
//                        switcher = sw;
//                        mockStrategy = new Mock<CircuitBreakerStrategy>(switcher);
//                        return mockStrategy.Object;
//                    }, cbConfig => cbConfig.BrakeWhenExceptionOccurs(ex => true)
//                        .OnClosed(() => state = State.Closed)
//                        .OnHalfOpen(() => state = State.HalfOpen)
//                        .OnOpen(ts => state = State.Open))));

//            mockStrategy.Setup(s => s.OperationFailedInClosed()).Callback(() => switcher.Open(TimeSpan.FromSeconds(.2))).Verifiable();

//            Assert.ThrowsException<InvalidOperationException>(() =>
//                policy.Execute((ctx, t) => throw new InvalidOperationException(), CancellationToken.None));

//            Assert.AreEqual(State.Open, state);

//            Assert.ThrowsException<CircuitOpenException>(() =>
//                policy.Execute((ctx, t) => { }, CancellationToken.None));

//            Thread.Sleep(TimeSpan.FromSeconds(.2));

//            mockStrategy.Setup(s => s.OperationFailedInHalfOpen()).Callback(() => switcher.Open(TimeSpan.FromSeconds(.2))).Verifiable();

//            Assert.ThrowsException<InvalidOperationException>(() =>
//                policy.Execute((ctx, t) => throw new InvalidOperationException(), CancellationToken.None));

//            Assert.AreEqual(State.Open, state);
//        }

//        [TestMethod]
//        public void CustomCircuitBreakerTests_Result_Closed_Open_HalfOpen_Then_Closed()
//        {
//            var state = State.Closed;

//            Mock<CircuitBreakerStrategy> mockStrategy = null;
//            ICircuitBreakerStateSwitcher switcher = null;
//            var policy = new BotPolicy<int>(config => config
//                .Configure(botConfig => botConfig
//                    .CustomCircuitBreaker(sw =>
//                    {
//                        switcher = sw;
//                        mockStrategy = new Mock<CircuitBreakerStrategy>(switcher);
//                        return mockStrategy.Object;
//                    }, cbConfig => cbConfig.BrakeWhenResultIs(r => r != 5)
//                        .OnClosed(() => state = State.Closed)
//                        .OnHalfOpen(() => state = State.HalfOpen)
//                        .OnOpen(ts => state = State.Open))));

//            mockStrategy.Setup(s => s.OperationFailedInClosed()).Callback(() => switcher.Open(TimeSpan.FromSeconds(.2))).Verifiable();

//            policy.Execute((ctx, t) => 6, CancellationToken.None);

//            Assert.AreEqual(State.Open, state);

//            Thread.Sleep(TimeSpan.FromSeconds(.2));

//            mockStrategy.Setup(s => s.OperationSucceededInHalfOpen()).Callback(() => switcher.Close()).Verifiable();

//            policy.Execute((ctx, t) =>
//            {
//                Assert.AreEqual(State.HalfOpen, state);
//                return 5;
//            }, CancellationToken.None);

//            Assert.AreEqual(State.Closed, state);
//        }

//        [TestMethod]
//        public void CustomCircuitBreakerTests_Result_Closed_Open_HalfOpen_Then_Closed_WithConfig()
//        {
//            var state = State.Closed;

//            Mock<CircuitBreakerStrategy> mockStrategy = null;
//            ICircuitBreakerStateSwitcher switcher = null;
//            var policy = new BotPolicy<int>(config => config
//                .Configure(botConfig => botConfig
//                    .CustomCircuitBreaker(sw =>
//                    {
//                        switcher = sw;
//                        mockStrategy = new Mock<CircuitBreakerStrategy>(switcher);
//                        return mockStrategy.Object;
//                    }, new CircuitBreakerConfiguration<int>()
//                        .BrakeWhenResultIs(r => r != 5)
//                        .OnClosed(() => state = State.Closed)
//                        .OnHalfOpen(() => state = State.HalfOpen)
//                        .OnOpen(ts => state = State.Open))));

//            mockStrategy.Setup(s => s.OperationFailedInClosed()).Callback(() => switcher.Open(TimeSpan.FromSeconds(.2))).Verifiable();

//            policy.Execute((ctx, t) => 6, CancellationToken.None);

//            Assert.AreEqual(State.Open, state);

//            Thread.Sleep(TimeSpan.FromSeconds(.2));

//            mockStrategy.Setup(s => s.OperationSucceededInHalfOpen()).Callback(() => switcher.Close()).Verifiable();

//            policy.Execute((ctx, t) =>
//            {
//                Assert.AreEqual(State.HalfOpen, state);
//                return 5;
//            }, CancellationToken.None);

//            Assert.AreEqual(State.Closed, state);
//        }

//        [TestMethod]
//        public void CustomCircuitBreakerTests_Result_Closed_Open_HalfOpen_Then_Open()
//        {
//            var state = State.Closed;

//            Mock<CircuitBreakerStrategy> mockStrategy = null;
//            ICircuitBreakerStateSwitcher switcher = null;
//            var policy = new BotPolicy<int>(config => config
//                .Configure(botConfig => botConfig
//                    .CustomCircuitBreaker(sw =>
//                    {
//                        switcher = sw;
//                        mockStrategy = new Mock<CircuitBreakerStrategy>(switcher);
//                        return mockStrategy.Object;
//                    }, cbConfig => cbConfig
//                        .BrakeWhenResultIs(r => r != 5)
//                        .OnClosed(() => state = State.Closed)
//                        .OnHalfOpen(() => state = State.HalfOpen)
//                        .OnOpen(ts => state = State.Open))));

//            mockStrategy.Setup(s => s.OperationFailedInClosed()).Callback(() => switcher.Open(TimeSpan.FromSeconds(.2))).Verifiable();

//            policy.Execute((ctx, t) => 6, CancellationToken.None);

//            Assert.AreEqual(State.Open, state);

//            Thread.Sleep(TimeSpan.FromSeconds(.2));

//            mockStrategy.Setup(s => s.OperationFailedInHalfOpen()).Callback(() => switcher.Open(TimeSpan.FromSeconds(.2))).Verifiable();

//            policy.Execute((ctx, t) => 6, CancellationToken.None);

//            Assert.AreEqual(State.Open, state);
//        }
//    }
//}
