using System.Threading.Tasks;

namespace Common
{
    public interface IConnector
    {
        public Task<ISyncConnection> Connect();
    }
}