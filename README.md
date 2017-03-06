# trybot [![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/trybot/master.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/trybot/branch/master) [![Travis CI build status](https://img.shields.io/travis/z4kn4fein/trybot/master.svg?label=travis-ci)](https://travis-ci.org/z4kn4fein/trybot) [![Coverage Status](https://coveralls.io/repos/github/z4kn4fein/trybot/badge.svg?branch=master)](https://coveralls.io/github/z4kn4fein/trybot?branch=master) [![Join the chat at https://gitter.im/z4kn4fein/trybot](https://img.shields.io/badge/gitter-join%20chat-1dce73.svg)](https://gitter.im/z4kn4fein/trybot?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![NuGet Version](https://buildstats.info/nuget/Trybot)](https://www.nuget.org/packages/Trybot/)
Trybot is a retry manager solution for .NET based projects. It can make your project more fault tolerant against external resources (persistent storage, network, etc.). 

##Features

 - Supports the re-execution of `Action`, `Func<Task>`, and `Func<Task<T>>`
 - Re-executions are performed asynchronously, they won't block the caller thread
 - Custom retry policies can be specified for handling on what circumstances you want to retry your actions
 - Custom retry strategies can be specified for handling, how your actions should be re-executed
 - Besides the exceptions you can set custom filters to control the re-executions
 - For `Func<Task<T>>` you can set a result filter
 - Subscribe to re-execution events
 - Cancellation support

##Supported platforms

 - .NET 4.5 and above
 - Windows 8/8.1/10
 - Windows Phone Silverlight 8/8.1
 - Windows Phone 8.1
 - Xamarin (Android/iOS/iOS Classic)
 - .NET Standard 1.0

##Retry policy
Using the `RetryManager` requires a properly configured `IRetryPolicy` implementation, which can decide, when you want to retry your operation.
```c#
public class FooRetryPolicy : IRetryPolicy
{
	public bool ShouldRetryAfter(Exception exception)
	{
		return true;
	}
}
```
> The example above will retry your operation when any kind of exception occurs.

Instantiation of `RetryManager`.
```c#
var retryManager = new RetryManager(new FooRetryPolicy());
```
####Retrying an `Action`
```c#
await retryManager.ExecuteAsync(() =>
{
	//some operation    
});
```
You can pass a cancellation token also.
```c#
var tokenSource = new CancellationTokenSource();
await retryManager.ExecuteAsync(() =>
{
	//some operation    
}, tokenSource.Token);
```
####Retrying a `Func<Task>`
```c#
await retryManager.ExecuteAsync(async() =>
{
	//some awaitable operation    
});
```
####Retrying a `Func<Task<T>>`
```c#
var result = await retryManager.ExecuteAsync(async() =>
{
	//some awaitable operation    
});
```
####Retry events
```c#
await retryManager.ExecuteAsync(() =>
{
	//some operation    
}, onRetryOccured: (attempt, nextDelay) =>
{
	Console.WriteLine($"{attempt}. attempt, waiting {nextDelay.TotalSeconds} seconds before the next retry!");
});
```
##Retry strategies
Custom retry strategies can be specified by passing a `RetryStrategy` implementation to the `ExecuteAsync()` function.
```c#
class FooRetryStrategy : RetryStartegy
{
	public FooRetryStrategy(int retryCount, TimeSpan delay)
           : base(retryCount, delay)
    {
    }
    
    protected override TimeSpan GetNextDelay(int currentAttempt)
    {
		//here you can calculate the next delay based on the current attempt 
		//or on the retry count and the initial delay
    }
}
```
Passing the custom strategy to the `ExecuteAsync()` function.
```c#
await retryManager.ExecuteAsync(() =>
{
	//some operation    
}, retryStartegy: new FooRetryStrategy(5, TimeSpan.FromSeconds(5)));
```
If you don't want to set your custom `RetryStrategy` on every `ExecuteAsync()` call, you can set the `RetryStrategy.DefaultRetryStrategy` static property as well, which's being used when the strategy parameter is null.
```c#
RetryStrategy.DefaultRetryStrategy = new FooRetryStrategy(5, TimeSpan.FromSeconds(5));
```
###Already implemented strategies

 - **FixedIntervalRetryStrategy** (it'll wait between the attempts always the same amount of time you specified at the instantiation)

	![fixed-small](https://cloud.githubusercontent.com/assets/13772020/11634019/93a4e4a0-9d0e-11e5-995d-4514e9d8a941.png)

 - **LinearRetryStartegy** (it'll calculate the wait time from the inital delay, multiplied by the attempt count)

	![linear-small](https://cloud.githubusercontent.com/assets/13772020/11633993/776a9f64-9d0e-11e5-9f4f-2ddd8177014d.png)

 - **SquareRetryStrategy** (it'll calculate the wait time from the squares of the inital delay's multiplication by the attempt count)

	![square-small](https://cloud.githubusercontent.com/assets/13772020/11633971/5da06ee2-9d0e-11e5-9510-d032e58b3818.png)

 - **CubicRetryStrategy** (it'll calculate the wait time based on the basic cubic function *[y = x3]* where *x* is the initial delay value multiplied by the attempt count)

	![cubic-small](https://cloud.githubusercontent.com/assets/13772020/11633946/403bbc62-9d0e-11e5-8bf9-2e17ed23cb8a.png)

##Filters
####Retry filter
A `Func<bool>` delegate can be set as a filter to determine what conditions must be met to retry an operation.
```c#
await retryManager.ExecuteAsync(() =>
{
	//some operation    
}, retryFilter: () => !state.IsValid());
```
> The filter above will continue the re-executions until the passed predicate is evaluated as `false`.

####Result filter for `Func<Task<T>>`
For the execution of a `Func<Task<T>>` you can specify a result filter which can check the result of the `Task<T>` and if it doesn't meet your criteria the `RetryManager` will re-execute your operation.
```c#
var result = await retryManager.ExecuteAsync(async() =>
{
	//some operation    
}, resultFilter: operationResult => !operationResult.IsValid);
```
