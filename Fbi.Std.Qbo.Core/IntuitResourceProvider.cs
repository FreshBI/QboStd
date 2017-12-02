using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Fbi.Std.Qbo.Core
{
    public abstract class IntuitResourceProvider
    {
        private readonly HttpClient httpClient;
        private readonly IntuitAuthClient authClient;

        public IntuitResourceProvider(HttpClient httpClient, IntuitAuthClient authClient)
        {
            this.httpClient = httpClient;
            this.authClient = authClient;
        }

        protected async Task<HttpResponseMessage> SendAsync(HttpMethod method, string uri)
        {
            var accessToken = await authClient.RefreshAccessToken();

            var request = new HttpRequestMessage(method, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization =
              new AuthenticationHeaderValue("Bearer", accessToken);

            return await httpClient.SendAsync(request);
        }
    }
}
