using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Common
{
    public interface IMenuUserSession : IAuthenticatedState, IDisposable
    {
        event Action ConnectionSevered;

        [CanBeNull] string PlayerId { get; }

        Task<SignInResult> SignIn(string playerId);

        void SignOut();

        Task<HostSessionResult> HostSession();

        Task<JoinSessionResult> JoinSession(string hostId);

        void IDisposable.Dispose() => SignOut();
    }

    public enum SignInResult
    {
        Success,
        UserNotFound
    }

    public enum HostSessionResult
    {
        Success,
        UnknownError
    }

    public enum JoinSessionResult
    {
        Success,
        HostNotFound,
        NotWelcome,
        SessionDoesNotExists
    }
}
