using System.Diagnostics;
using System.Threading;
using Common;
using Debug = UnityEngine.Debug;
using RiptideClient = Riptide.Client;

namespace Web.Sync
{
    public partial class SyncConnection : ISyncConnection
    {
        private readonly RiptideClient _client;
        private readonly Stopwatch _syncWatch;
        private readonly CancellationTokenSource _cancellation;

        private SyncConnection(RiptideClient client)
        {
            _client = client;
            _syncWatch = new Stopwatch();
            _cancellation = new CancellationTokenSource();
        }

        public void Dispose()
        {
            _cancellation.Cancel();
            _client.Disconnect();
            Debug.Log($"Dispose {nameof(SyncConnection)}");
        }
    }
}