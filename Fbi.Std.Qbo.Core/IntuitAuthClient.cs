using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Fbi.Std.Qbo.Core
{ 
    public class IntuitAuthClient
    {
        const string refreshTokenStorageKey = "RefreshToken";
        public string preconfiggedHashedClientIdClientSecretAuthCode = "UTBQMEZqR0tyYWNQbGt3RUpxeFk3QmdrS2hIcDN0RXdTUGtDUW5NR0tjdjRWbzd5VVM6R0IzU2hnaEpBQk1GTENxeWl3Z1NPRVNWeGJLUjJFMzhUWnh4Tmtzdg==";
        private string oauthUri = "https://oauth.platform.intuit.com";
        readonly HttpClient client;
        readonly ISecureStorage secureStorage;

        //This Auth Client is For Production use
        public IntuitAuthClient(string clientId, string sharedSecret, ISecureStorage secureStorage)
        {
            this.secureStorage = secureStorage;
            client = new HttpClient() { BaseAddress = new Uri(oauthUri) };
            client.DefaultRequestHeaders.Authorization =
              new AuthenticationHeaderValue("Basic", Base64EncodeClientSecret(clientId, sharedSecret));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //This Auth Client is Preconfigged to the test company
        //Not Used in Production
        public IntuitAuthClient(ISecureStorage secureStorage)
        {
            this.secureStorage = secureStorage;
            client = new HttpClient() { BaseAddress = new Uri(oauthUri) };
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", preconfiggedHashedClientIdClientSecretAuthCode);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //This creates the hashed authCredsForUserClient from Client ID and the Shared secret. Not for testing use.
        //Not used in Production
        private static string Base64EncodeClientSecret(string clientId, string sharedSecret)
        {
            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(
                    $"{clientId}:{sharedSecret}"));
        }

        //Not Used in Production
        public async Task<string> GetAccessToken(string accessCode)
        {
            var data = new Dictionary<string, string>
                  {
                    {"code", accessCode },
                    {"grant_type", "authorization_code" },
                    {"redirect_uri", "https://developer.intuit.com/v2/OAuth2Playground/RedirectUrl" }
                  };

            return await GetAccessToken(data);
        }


        public async Task<string> RefreshAccessToken()
        {
            var refreshToken = secureStorage.Retrieve<string>(refreshTokenStorageKey);

            var data = new Dictionary<string, string>
                  {
                    {"grant_type", "refresh_token" },
                    {"refresh_token", refreshToken }
                  };

            return await GetAccessToken(data);
        }

        private async Task<string> GetAccessToken(IDictionary<string, string> data)
        {
            var getAccessTokenResponse = await client.PostAsync("oauth2/v1/tokens/bearer", new FormUrlEncodedContent(data));

            var token = await getAccessTokenResponse.Content.ReadAsAsync<Token>();

            secureStorage.Store(refreshTokenStorageKey, token.RefreshToken);

            return token.AccessToken;
        }
    }
}
