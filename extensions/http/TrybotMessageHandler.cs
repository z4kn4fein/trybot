using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Trybot.Extensions.Http
{
    public class TrybotMessageHandler : DelegatingHandler
    {
        private readonly IBotPolicy<HttpResponseMessage> botPolicy;

        public TrybotMessageHandler(Action<IBotPolicyBuilder<HttpResponseMessage>> configuratorAction)
        {
            this.botPolicy = new BotPolicy<HttpResponseMessage>();
            this.botPolicy.Configure(configuratorAction);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationdId = request.GetCorrelationId();
            return await this.botPolicy.ExecuteAsync((req, ctx, token) =>
                base.SendAsync(req, token), correlationdId, request, cancellationToken);
        }
    }
}
