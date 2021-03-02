# Trybot 
[![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/trybot/master.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/trybot/branch/master) [![Travis CI build status](https://img.shields.io/travis/z4kn4fein/trybot/master.svg?label=travis-ci)](https://travis-ci.org/z4kn4fein/trybot) [![Tests](https://img.shields.io/appveyor/tests/pcsajtai/trybot-1453m/master.svg)](https://ci.appveyor.com/project/pcsajtai/trybot-1453m/build/tests) [![coverage](https://codecov.io/gh/z4kn4fein/trybot/branch/master/graph/badge.svg)](https://codecov.io/gh/z4kn4fein/trybot) [![Sourcelink](https://img.shields.io/badge/sourcelink-enabled-brightgreen.svg)](https://github.com/dotnet/sourcelink)

Trybot is a transient fault handling framework including such resiliency solutions as **Retry**, **Timeout**, **Fallback**, **Rate limit**, and **Circuit Breaker**. The framework is extendable with **custom, user-defined** bots.

Github (stable) | NuGet (stable) | MyGet (pre-release)
--- | --- | ---
[![Github release](https://img.shields.io/github/release/z4kn4fein/trybot.svg)](https://github.com/z4kn4fein/trybot/releases) | [![NuGet Version](https://buildstats.info/nuget/trybot)](https://www.nuget.org/packages/trybot/) | [![MyGet package](https://img.shields.io/myget/pcsajtai/v/trybot.svg?label=myget)](https://www.myget.org/feed/pcsajtai/package/nuget/trybot)

## Bots
- **[Retry](https://github.com/z4kn4fein/trybot/wiki/Retry)** - Allows configuring auto re-execution of an operation based on exceptions it throws or on its return value.

- **[Timeout](https://github.com/z4kn4fein/trybot/wiki/Timeout)** - Ensures that the caller won't have to wait indefinitely for an operation to finish by setting a maximum time range within the given operation should be executed.

- **[Fallback](https://github.com/z4kn4fein/trybot/wiki/Fallback)** - Handles faults by executing an alternative operation when the original one is failing, also provides the ability to produce an alternative result value when the actual operation is not able to do it.

- **[Circuit breaker](https://github.com/z4kn4fein/trybot/wiki/Circuit-breaker)** - When the number of failures exceeds a given threshold, this bot prevents the continuous re-execution of the failing operation by blocking the traffic for a configured amount of time. This usually could give some break to the remote resource to heal itself properly.

- **[Rate limit](https://github.com/z4kn4fein/trybot/wiki/Rate-limit)** - Controls the rate of the operations by specifying a maximum amount of executions within a given time window.

## Supported Platforms

 - .NET 4.5 and above
 - .NET Core
 - Mono
 - Universal Windows Platform
 - Xamarin (Android/iOS/Mac)
 - Unity

## Usage
During the configuration of a bot policy, you can chain different bots to each other.

```c#
policy.Configure(policyConfig => policyConfig
    .CircuitBreaker(circuitBreakerConfig => circuitBreakerConfig
        .DurationOfOpen(TimeSpan.FromSeconds(10))
        .BrakeWhenExceptionOccurs(exception => exception is HttpRequestException),
            strategyConfig => strategyConfig
                .FailureThresholdBeforeOpen(5)
                .SuccessThresholdInHalfOpen(2))

    .Retry(retryConfig => retryConfig
        .WithMaxAttemptCount(5)
        .WhenExceptionOccurs(exception => exception is HttpRequestException)
        .WaitBetweenAttempts((attempt, exception) => 
        {
            if(exception is CircuitOpenException cbException)
                return TimeSpan.FromSeconds(cbException.OpenDuration);

            return TimeSpan.FromSeconds(Math.Pow(2, attempt);
        })))

    .Timeout(timeoutConfig => timeoutConfig
        .After(TimeSpan.FromSeconds(120))));
```
> The handling order of the given operation would be the same as the configuration order from top to bottom. That means in the example above that the circuit breaker will try to execute the given operation first, then if it fails, the retry bot will start to re-execute it until the timeout bot is not signaling a cancellation.

Then you can execute the configured policy:

- With cancellation:

    ```c#
    var tokenSource = new CancellationTokenSource();
    policy.Execute((context, cancellationToken) => DoSomeCancellableOperation(cancellationToken), tokenSource.Token);
    ```

- With a custom correlation id:

    ```c#
    var correlationId = Guid.NewGuid();
    policy.Execute((context, cancellationToken) => DoSomeOperationWithCorrelationId(context.CorrelationId), correlationId);
    ```
    > Without setting a custom correlation id, the framework will always generate a unique one for every policy execution.

- Synchronously:

    ```c#
    // Without lambda parameters
    policy.Execute(() => DoSomeOperation());

    // Or with lambda parameters
    policy.Execute((context, cancellationToken) => DoSomeOperation());
    ```

- Asynchronously:

    ```c#
    // Without lambda parameters
    await policy.ExecuteAsync(() => DoSomeAsyncOperation());

    // Or with lambda parameters
    await policy.ExecuteAsync((context, cancellationToken) => DoSomeAsyncOperation());
    ```

> You can also create your custom bots as described [here](https://github.com/z4kn4fein/trybot/wiki/Custom-bots).

## Contact & Support
- [![Join the chat at https://gitter.im/z4kn4fein/trybot](https://img.shields.io/gitter/room/z4kn4fein/trybot.svg)](https://gitter.im/z4kn4fein/trybot)
- Create an [issue](https://github.com/z4kn4fein/trybot/issues) for bug reports, feature requests, or questions.
- Add a ⭐️ to support the project!

## Extensions
- ASP.NET Core 
  - [Trybot.Extensions.Http](https://github.com/z4kn4fein/trybot-extensions-http)
- Other
  - [Distributed Circuit Breaker simulation](https://github.com/z4kn4fein/trybot/tree/master/sandbox/trybot.distributedcb)

## Documentation
- [Wiki](https://github.com/z4kn4fein/trybot/wiki)
- [Resiliency patterns](https://docs.microsoft.com/en-us/azure/architecture/patterns/category/resiliency)
