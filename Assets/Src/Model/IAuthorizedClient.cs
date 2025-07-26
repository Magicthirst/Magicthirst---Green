using System;
using System.Threading.Tasks;
using Common;

namespace Model
{
    public interface IAuthorizedClient : IDisposable
    {
        event Action ConnectionSevered;

        Task<IConnector> Host();

        Task<(IConnector, JoinSessionResult)> Join(string hostId);

        void Exit();

        void IDisposable.Dispose() => Exit();
    }
}
