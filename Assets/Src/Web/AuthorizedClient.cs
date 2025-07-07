using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Web
{
    public class AuthorizedClient
    {
        public event Action ConnectionSevered;

        public string Token { get; private set; }

        private volatile bool _running = false;
        private readonly HttpClient _http;

        private AuthorizedClient(HttpClient http, string token)
        {
            _http = http;
            Token = token;
        }

        public static async Task<ResultOrFail<AuthorizedClient>> Launch
        (
            Config config,
            Credentials credentials
        )
        {
            var http = new HttpClient { BaseAddress = new Uri(config.Url) };

            var tokenOrFail = await GetToken(http, credentials);
            if (tokenOrFail is ResultOrFail<string>.UserNotFound)
            {
                return new ResultOrFail<AuthorizedClient>.UserNotFound();
            }

            var token = (tokenOrFail as ResultOrFail<string>.Ok)!.Value;

            var auth = new AuthorizedClient(http, token);
            _ = auth.LaunchRefresh(config.RefreshInterval);

            return new ResultOrFail<AuthorizedClient>.Ok(auth);
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        private static async Task<ResultOrFail<string>> GetToken
        (
            HttpClient http,
            Credentials credentials
        )
        {
            var login = await http.GetAsync($"/hosts/{credentials.Uuid}/access_token");
            if (!login.IsSuccessStatusCode)
            {
                return new ResultOrFail<string>.UserNotFound();
            }

            var jsonString = await login.Content.ReadAsStringAsync();
            var result = JsonUtility.FromJson<TokenResult>(jsonString);

            return new ResultOrFail<string>.Ok(result.token);
        }

        private async Task LaunchRefresh(TimeSpan interval)
        {
            while (true)
            {
                await Task.Delay(interval);

                var login = await _http.SendAsync(RenewRequest());
                if (!login.IsSuccessStatusCode)
                {
                    ConnectionSevered?.Invoke();
                    break;
                }

                var jsonString = await login.Content.ReadAsStringAsync();
                var result = JsonUtility.FromJson<TokenResult>(jsonString);

                Token = result.token;
            }
        }

        private HttpRequestMessage RenewRequest() => new()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("/hosts/access_token/renew"),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", Token) }
        };

        [Serializable]
        public record Config(string Url, TimeSpan RefreshInterval);

        public record Credentials(string Uuid);

        public record ResultOrFail<T>
        {
            private ResultOrFail() {}

            public sealed record Ok(T Value) : ResultOrFail<T>;

            public sealed record UserNotFound : ResultOrFail<T>;
        }

        [Serializable]
        // ReSharper disable once InconsistentNaming
        private sealed record TokenResult(string token);
    }
}
