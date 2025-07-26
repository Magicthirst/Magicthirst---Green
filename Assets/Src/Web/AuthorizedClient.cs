using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Model;
using UnityEngine;
using Web.Sync;
using Web.Util;

namespace Web
{
    public partial class AuthorizedClient : IAuthorizedClient
    {
        public event Action ConnectionSevered;

        private string _token;
        private readonly string _playerId;
        private readonly ClientConfig _config;

        private readonly CancellationTokenSource _cancellationSource;
        private readonly HttpClient _http;

        private AuthorizedClient(HttpClient http, string token, string playerId, ClientConfig config)
        {
            _http = http;
            _token = token;
            _playerId = playerId;
            _config = config;
            _cancellationSource = new CancellationTokenSource();
        }

        internal static AuthorizedClient Launch(ClientConfig config, string token, string playerId)
        {
            var http = new HttpClient { BaseAddress = new Uri(config.GatewayUrl) };
            var client = new AuthorizedClient(http, token, playerId, config);
            _ = client.LaunchRefresh(config.TokenRefreshInterval);

            return client;
        }

        public async Task<IConnector> Host()
        {
            var response = await _http.SendAsync(new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/sessions", UriKind.Relative),
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _token) },
                Content = JsonContent.Create(new { host = _playerId })
            }, _cancellationSource.Token);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var content = await response.Content.ReadJsonContentBy(new { session_id = 0, source_of_truth_key = "" });

                return new HostConnector
                (
                    serverUrl: response.Headers.GetValues("Location").Single(),
                    sessionId: content.session_id,
                    playerId: _playerId,
                    sourceOfTruthKey: content.source_of_truth_key,
                    config: _config
                );
            }

            Debug.LogError(response);
            throw new NotImplementedException();
        }

        public async Task<(IConnector, JoinSessionResult)> Join(string hostId)
        {
            var response = await _http.SendAsync(new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/sessions/players", UriKind.Relative),
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _token) },
                Content = JsonContent.Create(new { host = hostId, guest = _playerId })
            }, _cancellationSource.Token);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    var content = await response.Content.ReadJsonContentBy(new { session_id = 0 });
                    var connector = new GuestConnector
                    (
                        serverUrl: response.Headers.GetValues("Location").Single(),
                        sessionId: content.session_id,
                        playerId: _playerId,
                        config: _config
                    );

                    return (connector, JoinSessionResult.Success);
                }
                case HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized:
                {
                    return (null, JoinSessionResult.UnknownError);
                }
                case HttpStatusCode.NotFound when response.Content.GetType().Name == "EmptyContent":
                {
                    return (null, JoinSessionResult.HostNotFound);
                }
                case HttpStatusCode.NotFound:
                {
                    return await response.Content.ReadAsStringAsync() switch
                    {
                        "no session" => (null, JoinSessionResult.SessionDoesNotExists),
                        "not welcome" => (null, JoinSessionResult.NotWelcome),
                        _ => throw new NotImplementedException()
                    };
                }
                default:
                {
                    Debug.LogError(response);
                    throw new NotImplementedException();
                }
            }
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
                        RequestUri = new Uri("/hosts/access_token/renew", UriKind.Relative),
                        Headers = { Authorization = new AuthenticationHeaderValue("Bearer", _token) }
                    }, _cancellationSource.Token);

                    if (!login.IsSuccessStatusCode)
                    {
                        ConnectionSevered?.Invoke();
                        break;
                    }

                    var result = await login.Content.ReadJsonContentBy(new { token = "" });
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

    internal class HostConnector : IConnector
    {
        private readonly string _serverUrl;
        private readonly int _sessionId;
        private readonly string _playerId;
        private readonly string _sourceOfTruthKey;
        private readonly ClientConfig _config;

        public HostConnector(string serverUrl, int sessionId, string playerId, string sourceOfTruthKey, ClientConfig config)
        {
            _serverUrl = serverUrl;
            _sessionId = sessionId;
            _playerId = playerId;
            _sourceOfTruthKey = sourceOfTruthKey;
            _config = config;
        }

        public async Task<ISyncConnection> Connect() =>
            await SyncConnection.LaunchOrNull(_serverUrl, _sessionId, _playerId, _config, _sourceOfTruthKey);
    }

    internal class GuestConnector : IConnector
    {
        private readonly string _serverUrl;
        private readonly int _sessionId;
        private readonly string _playerId;
        private readonly ClientConfig _config;

        public GuestConnector(string serverUrl, int sessionId, string playerId, ClientConfig config)
        {
            _serverUrl = serverUrl;
            _sessionId = sessionId;
            _playerId = playerId;
            _config = config;
        }

        public async Task<ISyncConnection> Connect() =>
            await SyncConnection.LaunchOrNull(_serverUrl, _sessionId, _playerId, _config);
    }
}
