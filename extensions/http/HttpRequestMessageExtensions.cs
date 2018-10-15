namespace System.Net.Http
{
    public static class HttpRequestMessageExtensions
    {
        private const string CorrelationIdKey = "TrybotCorrelationIdKey";

        public static object GetCorrelationId(this HttpRequestMessage message) =>
            message.Properties.TryGetValue(CorrelationIdKey, out var value) ? value : null;

        public static object SetCorrelationId(this HttpRequestMessage message, object correlationId) =>
            message.Properties[CorrelationIdKey] = correlationId;
    }
}
