using System.Threading.Tasks;

namespace Model
{
    public interface IConnector
    {
        public Task<ISyncConnection> Connect();
    }
}