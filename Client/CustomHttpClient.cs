using Client.Authentication;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class CustomHttpClient : HttpClient
    {
        public CustomHttpClient(HttpMessageHandler handler) : base(handler)
        {
        }

        internal async Task<TResponse> PostObjectAsync<TResponse>(string endpoint,
            object item)
        {

            string json = JsonConvert.SerializeObject(item);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpResponseMessage response = await this.PostAsync(endpoint, content))
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // TODO: Maybe violates "do not use exceptions for flow control"?
                    throw new ServiceUnauthorizedException(response.ReasonPhrase, response.Headers.Location);
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string message = await response.Content.ReadAsStringAsync();

                    throw new InvalidOperationException(message);
                }

                string result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(result);
            }
        }

        internal async Task<TResponse> PostAsync<TResponse>(string endpoint)
        {
            using (HttpResponseMessage response = await this.PostAsync(endpoint, null))
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // TODO: Maybe violates "do not use exceptions for flow control"?
                    throw new ServiceUnauthorizedException(response.ReasonPhrase, response.Headers.Location);
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string message = await response.Content.ReadAsStringAsync();

                    throw new InvalidOperationException(message);
                }

                string result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(result);
            }
        }

        internal async Task PostAsync(string endpoint)
        {
            using (HttpResponseMessage response = await this.PostAsync(endpoint, null))
            {

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // TODO: Maybe violates "do not use exceptions for flow control"?
                    throw new ServiceUnauthorizedException(response.ReasonPhrase, response.Headers.Location);
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string message = await response.Content.ReadAsStringAsync();

                    throw new InvalidOperationException(message);
                }
            }
        }
    }
}
