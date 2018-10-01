# trybot [![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/trybot/master.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/trybot/branch/master) [![Travis CI build status](https://img.shields.io/travis/z4kn4fein/trybot/master.svg?label=travis-ci)](https://travis-ci.org/z4kn4fein/trybot) [![Tests](https://img.shields.io/appveyor/tests/pcsajtai/trybot-1453m/master.svg)](https://ci.appveyor.com/project/pcsajtai/trybot-1453m/build/tests) [![Coverage Status](https://img.shields.io/codecov/c/github/z4kn4fein/trybot.svg)](https://codecov.io/gh/z4kn4fein/trybot) [![Join the chat at https://gitter.im/z4kn4fein/stashbox](https://img.shields.io/gitter/room/z4kn4fein/trybot.svg)](https://gitter.im/z4kn4fein/trybot?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Slack](https://img.shields.io/badge/chat-on%20slack-orange.svg?style=flat)](https://pcsajtai-dev-slack-in.herokuapp.com/)

Trybot is a lock-free (but thread-safe) transient fault handling framework including such built-in bots as [Retry](#retry), [Timeout](#timeout), [Fallback](#fallback) and [Circuit Breaker](#circuit-breaker). The framework is extendable with [custom, user-defined](#custom-policies) policies as well.

Github (stable) | NuGet (stable) | MyGet (pre-release) 
--- | --- | ---
[![Github release](https://img.shields.io/github/release/z4kn4fein/trybot.svg)](https://github.com/z4kn4fein/trybot/releases) | [![NuGet Version](https://buildstats.info/nuget/trybot)](https://www.nuget.org/packages/trybot/) | [![MyGet package](https://img.shields.io/myget/pcsajtai/v/trybot.svg?label=myget)](https://www.myget.org/feed/pcsajtai/package/nuget/trybot)

## Supported platforms

 - .NET 4.5 and above
 - Windows 8/8.1/10
 - Windows Phone Silverlight 8/8.1
 - Windows Phone 8.1
 - Xamarin (Android/iOS/iOS Classic)
 - .NET Standard 1.0/2.0

## Retry

- Configuration
    - For operations **without** a return value
        ```c#
        // Create a new bot policy
        var policy = new BotPolicy();

        // Configure the policy to retry failed operations
        policy.Configure(policyConfig => policyConfig
            .Retry(retryConfig => retryConfig
                // Set the maximum retry count
                .WithMaxAttemptCount(5)
                // Set the predicate which will used to decide that whether an exception should be handled or not
                .WhenExceptionOccurs(exception => exception is HttpRequestException)))
                // Set the delegate function used to calculate the amount of time to wait between the retry attempts
                .WaitBetweenAttempts((attempt, exception) => TimeSpan.FromSeconds(5))
                // (optional) Set a callback delegate which will be invoked when the given operation is being re-executed
                .OnRetry((exception, attemptContext) => Console.WriteLine($"{attemptContext.CurrentAttempt}. retry attempt, waiting {attemptContext.CurrentDelay}"));
        ```
    - For operations **with** a return value
        ```c#
        // Create a new bot policy
        var policy = new BotPolicy<HttpResponseMessage>();

        // Configure the policy to retry failed operations
        policy.Configure(policyConfig => policyConfig
            .Retry(retryConfig => retryConfig
                // Set the maximum retry count
                .WithMaxAttemptCount(5)
                // Set the predicate which will used to decide that whether an exception should be handled or not
                .WhenExceptionOccurs(exception => exception is HttpRequestException)))
                // Set the predicate which will used to decide that whether the result of the operation should be handled or not
                .WhenResultIs(result => result.StatusCode != HttpStatusCode.Ok)))
                // Set the delegate function used to calculate the amount of time to wait between the retry attempts
                .WaitBetweenAttempts((attempt, result, exception) => TimeSpan.FromSeconds(5))
                // (optional) Set a callback delegate which will be invoked when the given operation is being re-executed
                .OnRetry((exception, result, attemptContext) => Console.WriteLine($"{attemptContext.CurrentAttempt}. retry attempt, waiting {attemptContext.CurrentDelay}, result: {result}"));
        ```
- Execute the configured policy
    - With cancellation
        ```c#
        var tokenSource = new CancellationTokenSource();
        policy.Execute((context, cancellationToken) => DoSomeCancellableAction(cancellationToken), tokenSource.Token);
        ```
    - With custom correlation id
        ```c#
        var correlationId = Guid.NewGuid();
        policy.Execute((context, cancellationToken) => DoSomeActionWithCorrelationId(context.CorrelationId), correlationId);
        ```
        > Without setting a custom correlation id, the framework will always generate a unique one for every policy execution.
    - Synchronously
        ```c#
        // Without lambda parameters
        policy.Execute(() => DoSomeAction());
        ```
    - Asynchronously
        ```c#
        // Without lambda parameters
        await policy.ExecuteAsync(() => DoSomeAction());
        ```

> When the number of re-executions are reaching the configured maximum attempt count, a custom `MaxRetryAttemptsReachedException` exception is being thrown.

## Timeout

## Fallback

## Circuit breaker