using System.Net;
using System.Text;

namespace GamerCore.CustomerSite.Tests.Utilities
{
    /// <summary>
    /// Simple mock HttpMessageHandler to simulate HttpClient responses.
    /// </summary>
    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _responseContent;

        public MockHttpMessageHandler(HttpStatusCode statusCode, string responseContent = "")
        {
            _statusCode = statusCode;
            _responseContent = responseContent;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(response);
        }
    }
}