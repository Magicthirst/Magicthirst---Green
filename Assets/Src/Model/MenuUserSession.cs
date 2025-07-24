using System;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Model.Exception;
using UnityEngine;

namespace Model
{
    public class MenuUserSession : IMenuUserSession
    {
        public event Action ConnectionSevered;

        public event Action<bool> IsAuthenticatedChanged;

        public bool IsAuthenticated => _client != null;

        [CanBeNull] private string _playerId;
        public string PlayerId
        {
            get => _playerId;
            private set
            {
                _playerId = value;
                IsAuthenticatedChanged?.Invoke(value == null);
            }
        }

        private readonly IClientAuthenticator _authenticator;
        private readonly Action<IConnector> _setConnector;
        private IAuthorizedClient _client = null;

        public MenuUserSession(IClientAuthenticator authenticator, Action<IConnector> setConnector)
        {
            _authenticator = authenticator;
            _setConnector = setConnector;
            ConnectionSevered = () => PlayerId = null;
        }

        public async Task<SignInResult> SignIn(string playerId)
        {
            IAuthorizedClient client;
            try
            {
                client = await _authenticator.SignIn(playerId);
                PlayerId = playerId;
                _client = client;
                _client.ConnectionSevered += OnConnectionSevered;
                return SignInResult.Success;
            }
            catch (UserNotFound)
            {
                return SignInResult.UserNotFound;
            }

            void OnConnectionSevered()
            {
                ConnectionSevered?.Invoke();
                client.ConnectionSevered -= OnConnectionSevered;
            }
        }

        public void SignOut()
        {
            PlayerId = null;
            _client?.Exit();
        }

        public async Task<HostSessionResult> HostSession()
        {
            if (!IsAuthenticated)
            {
                throw new InvalidOperationException("User is not authenticated to be able to host a session");
            }

            try
            {
                _setConnector(await _client.Host());
                return HostSessionResult.Success;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return HostSessionResult.UnknownError;
            }
        }

        public Task<JoinSessionResult> JoinSession(string hostId)
        {
            throw new NotImplementedException();
        }
    }
}
