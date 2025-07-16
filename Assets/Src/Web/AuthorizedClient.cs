using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Model;
using UnityEngine;
using Web.Dto;

namespace Web
{
    public partial class AuthorizedClient : IAuthorizedClient
    {
        public event Action ConnectionSevered;

        private string _token;

        private readonly CancellationTokenSource _cancellationSource;
        private readonly HttpClient _http;

        private AuthorizedClient(HttpClient http, string token)
        {
            _http = http;
            _token = token;
            _cancellationSource = new CancellationTokenSource();
        }

        internal static AuthorizedClient Launch(ClientConfig config, string token)
        {
            var http = new HttpClient { BaseAddress = new Uri(config.GatewayUrl) };
            var client = new AuthorizedClient(http, token);
            _ = client.LaunchRefresh(config.RefreshInterval);

            return client;
        }

        public Task<IConnector> Host()
        {
            throw new NotImplementedException();
        }

        public Task<IConnector> Join(string hostId)
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            _cancellationSource.Cancel();
            // TODO request to delete the authentication token from the server
            _http.Dispose();
        }

        private async Task LaunchRefresh(TimeSpan interval)
        {
            try
            {
                while (!_cancellationSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(interval, _cancellationSource.Token);

                    var login = await _http.SendAsync(new()
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri("/hosts/access_token/renew"),
                        Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _token) }
                    }, _cancellationSource.Token);

                    if (!login.IsSuccessStatusCode)
                    {
                        ConnectionSevered?.Invoke();
                        break;
                    }

                    var jsonString = await login.Content.ReadAsStringAsync();
                    var result = JsonUtility.FromJson<TokenResultDto>(jsonString);

                    _token = result.token;
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogError($"An error occurred during token refresh: {e}");
                ConnectionSevered?.Invoke();
            }
        }
    }
}
