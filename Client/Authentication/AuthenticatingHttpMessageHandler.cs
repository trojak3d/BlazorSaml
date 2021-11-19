using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Client.Authentication {
    public class AuthenticatingHttpMessageHandler : DelegatingHandler {
        public AuthenticatingHttpMessageHandler(HttpMessageHandler innerHandler)
            : base(innerHandler) {
        }

        public NavigationManager NavigationManager { get; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken) {

            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

#if DEBUG
            Console.WriteLine($"Sending request: {request.Method} {request.RequestUri}");
#endif

            HttpResponseMessage response;
            try {
                response = await base.SendAsync(request, cancellationToken);
            } catch (Exception ex) {
#if DEBUG
                Console.WriteLine($"Exception during request: {ex}");
#endif
                throw;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized) {
                // TODO: Maybe violates "do not use exceptions for flow control"?
                throw new ServiceUnauthorizedException(response.ReasonPhrase, response.Headers.Location);
            }

#if DEBUG
            Console.WriteLine($"Got response: {(int)response.StatusCode} {response.ReasonPhrase} {request.RequestUri}");
#endif

            return response;
        }
    }
}
