using System;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Model.Exception;

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
        private IAuthorizedClient _client = null;

        public MenuUserSession(IClientAuthenticator authenticator)
        {
            _authenticator = authenticator;
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
            _client?.Exit();
        }

        public Task<HostSessionResult> HostSession()
        {
            throw new NotImplementedException();
        }

        public Task<JoinSessionResult> JoinSession(string hostId)
        {
            throw new NotImplementedException();
        }
    }
}
