using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Riptide;
using Riptide.Utils;
using UnityEngine;

namespace Web.Sync
{
    public partial class SyncConnection
    {
        [ItemCanBeNull]
        public static async Task<SyncConnection> LaunchOrNull
        (
            string url,
            int sessionId,
            string playerId,
            ClientConfig config,
            [CanBeNull] string sourceOfTruthKey = null
        )
        {
            RiptideLogger.Initialize(Debug.Log, true);
            var cancellationToken = new CancellationTokenSource(config.ConnectionTimeout).Token;

            var client = new Client();

            var connectedToServer = await client.ConnectSequentially(url, config.SyncLoopInterval, cancellationToken);
            if (!connectedToServer)
            {
                return null;
            }

            var connectedToSession = await client.SendWithResponse
            (
                ConnectToSessionMessage(sessionId, playerId, sourceOfTruthKey),
                map: (mark, _) => mark.HasFlag(MessageMark.Accepted),
                refreshInterval: config.SyncLoopInterval,
                cancellationToken: cancellationToken
            );

            if (!connectedToSession)
            {
                client.Disconnect();
                return null;
            }

            var connection = new SyncConnection(client);
            _ = connection.RunSyncLoop(config.SyncLoopInterval);

            return connection;
        }

        private static Message ConnectToSessionMessage(int sessionId, string playerId,
            [CanBeNull] string sourceOfTruthKey = null)
        {
            var message = Message
                .Create(MessageSendMode.Reliable, MessageMark.Connected)
                .AddInt(sessionId)
                .AddString(playerId);

            if (sourceOfTruthKey != null)
            {
                message.AddString(sourceOfTruthKey);
            }

            return message;
        }
    }
}