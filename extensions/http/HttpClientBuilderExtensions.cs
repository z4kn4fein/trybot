using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using Trybot.Utils;

namespace Trybot.Extensions.Http
{
    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddTrybotPolicy(this IHttpClientBuilder builder, IBotPolicy<HttpResponseMessage> policy)
        {
            Shield.EnsureNotNull(policy, nameof(policy));

            return builder.AddHttpMessageHandler(() => new TrybotMessageHandler(policy));
        }

        public static IHttpClientBuilder AddTrybotPolicy(this IHttpClientBuilder builder, Action<IBotPolicyBuilder<HttpResponseMessage>> policyBuilder)
        {
            Shield.EnsureNotNull(policyBuilder, nameof(policyBuilder));

            return builder.AddHttpMessageHandler(() => new TrybotMessageHandler(policyBuilder));
        }
    }
}
