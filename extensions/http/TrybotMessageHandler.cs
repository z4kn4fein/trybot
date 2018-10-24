using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Trybot.Utils;

namespace Trybot.Extensions.Http
{
    public class TrybotMessageHandler : DelegatingHandler
    {
        protected readonly IBotPolicy<HttpResponseMessage> BotPolicy;

        public TrybotMessageHandler(Action<IBotPolicyBuilder<HttpResponseMessage>> policyBuilder)
        {
            Shield.EnsureNotNull(policyBuilder, nameof(policyBuilder));

            this.BotPolicy = new BotPolicy<HttpResponseMessage>();
            this.BotPolicy.Configure(policyBuilder);
        }

        public TrybotMessageHandler(IBotPolicy<HttpResponseMessage> botPolicy)
        {
            Shield.EnsureNotNull(botPolicy, nameof(botPolicy));

            this.BotPolicy = botPolicy;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationdId = request.GetCorrelationId();
            return await this.BotPolicy.ExecuteAsync((req, ctx, token) =>
                this.WrappedSendAsync(req, token), correlationdId, request, cancellationToken);
        }

        protected virtual Task<HttpResponseMessage> WrappedSendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            base.SendAsync(request, cancellationToken);
    }
}
