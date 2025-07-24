using System.Threading;
using Common;
using RiptideClient = Riptide.Client;

namespace Web.Sync
{
    public partial class SyncConnection : ISyncConnection
    {
        private readonly RiptideClient _client;
        private readonly CancellationTokenSource _cancellation;

        private SyncConnection(RiptideClient client)
        {
            _client = client;
            _cancellation = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _cancellation.Cancel();
            _client.Disconnect();
        }
    }
}