using System;
using System.Threading.Tasks;

namespace Model
{
    public interface IAuthorizedClient
    {
        public event Action ConnectionSevered;

        public Task<IConnector> Host();

        public Task<IConnector> Join(string hostId);

        public void Exit();
    }
}
