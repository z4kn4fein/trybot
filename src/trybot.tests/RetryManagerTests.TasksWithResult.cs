using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Tests
{
    public partial class RetryManagerTests
    {
        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExecuteAsync_FuncTaskWithResult_WithoutFilter_Exception()
        {
            this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetException(new Exception());
                return taskCompletionSource.Task;
            }, CancellationToken.None, this.executionPolicy).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExecuteAsync_FuncTaskWithResult_WithoutFilter_Exception_WithoutCancellationToken()
        {
            this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetException(new Exception());
                return taskCompletionSource.Task;
            }, this.executionPolicy).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExecuteAsync_FuncTaskWithResult_WithoutFilter_CancellationToken_Exception()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var task = this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetException(new Exception());
                return taskCompletionSource.Task;
            }, cancellationTokenSource.Token, this.executionPolicy);
            cancellationTokenSource.Cancel();
            task.Wait();
        }

        [TestMethod]
        public void ExecuteAsync_FuncTaskWithResult_WithoutFilter()
        {
            try
            {
                this.retryManager.ExecuteAsync(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetException(new Exception());
                    return taskCompletionSource.Task;
                }, CancellationToken.None, this.executionPolicy).Wait();
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.Counter);
            }
        }

        [TestMethod]
        public void ExecuteAsync_FuncTaskWithResult_WithFilter_False()
        {
            try
            {
                this.retryManager.ExecuteAsync(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetException(new Exception());
                    return taskCompletionSource.Task;
                }, CancellationToken.None, this.executionPolicy, () => false).Wait();
            }
            catch (Exception)
            {
                Assert.AreEqual(5, this.executionPolicy.Counter);
            }
        }

        [TestMethod]
        public void ExecuteAsync_FuncTaskWithResult_WithoutFilter_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetException(new Exception());
                    return taskCompletionSource.Task;
                }, cancellationTokenSource.Token, this.executionPolicy);
                cancellationTokenSource.Cancel();
                task.Wait();
            }
            catch (Exception)
            {

                Assert.IsTrue(this.executionPolicy.Counter < 5);
            }
        }

        [TestMethod]
        public void ExecuteAsync_FuncTaskWithResult_WithFilter_True_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetException(new Exception());
                    return taskCompletionSource.Task;
                }, cancellationTokenSource.Token, this.executionPolicy, () => true);
                cancellationTokenSource.Cancel();
                task.Wait();
            }
            catch (Exception)
            {

                Assert.AreEqual(0, this.executionPolicy.Counter);
            }
        }

        [TestMethod]
        public void ExecuteAsync_FuncTaskWithResult_WithFilter_False_CancellationToken()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = this.retryManager.ExecuteAsync(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetException(new Exception());
                    return taskCompletionSource.Task;
                }, cancellationTokenSource.Token, this.executionPolicy, () => false);
                cancellationTokenSource.Cancel();
                task.Wait();
            }
            catch (Exception)
            {

                Assert.IsTrue(this.executionPolicy.Counter < 5);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ExecuteAsync_FuncTaskWithResult_WithFilter_True_Exception()
        {
            this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetException(new Exception());
                return taskCompletionSource.Task;
            }, CancellationToken.None, this.executionPolicy, () => true).Wait();
        }

        [TestMethod]
        public void ExecuteAsync_FuncTaskWithResult_WithFilter_True()
        {
            try
            {
                this.retryManager.ExecuteAsync(() =>
                {
                    var taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetException(new Exception());
                    return taskCompletionSource.Task;
                }, CancellationToken.None, this.executionPolicy, () => true).Wait();
            }
            catch (Exception)
            {
                Assert.AreEqual(0, this.executionPolicy.Counter);
            }
        }

        [TestMethod]
        public void ExecuteAsync_FuncTaskWithResult_WithoutRetry()
        {
            this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetResult(null);
                return taskCompletionSource.Task;
            }, CancellationToken.None, this.executionPolicy).Wait();
            Assert.AreEqual(0, this.executionPolicy.Counter);
        }

        [TestMethod]
        public void ExecuteAsync_FuncTaskWithResult_WithResultFilter()
        {
            var functionResult = this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.SetResult(true);
                return taskCompletionSource.Task;
            }, CancellationToken.None, this.executionPolicy, resultFilter: result => result).Result;
            Assert.AreEqual(5, this.executionPolicy.Counter);
            Assert.IsTrue(functionResult);
        }

        [TestMethod]
        public void ExecuteAsync_FuncTaskWithResult_WithResultFilter_WithoutCancellationToken()
        {
            var functionResult = this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.SetResult(true);
                return taskCompletionSource.Task;
            }, this.executionPolicy, resultFilter: result => result).Result;
            Assert.AreEqual(5, this.executionPolicy.Counter);
            Assert.IsTrue(functionResult);
        }

        [TestMethod]
        public void ExecuteAsync_FuncTaskWithResult_WithResultFilter_False()
        {
            var functionResult = this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.SetResult(true);
                return taskCompletionSource.Task;
            }, CancellationToken.None, this.executionPolicy, resultFilter: result => !result).Result;
            Assert.AreEqual(0, this.executionPolicy.Counter);
            Assert.IsTrue(functionResult);
        }

        [TestMethod]
        public void ExecuteAsync_FuncTaskWithResult_WithResultFilter_False_WithoutCancellationToken()
        {
            var functionResult = this.retryManager.ExecuteAsync(() =>
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                taskCompletionSource.SetResult(true);
                return taskCompletionSource.Task;
            }, this.executionPolicy, resultFilter: result => !result).Result;
            Assert.AreEqual(0, this.executionPolicy.Counter);
            Assert.IsTrue(functionResult);
        }
    }
}
