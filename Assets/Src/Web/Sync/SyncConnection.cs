using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Riptide;
using Riptide.Utils;
using UnityEngine;
using Web.Util;
using Debug = UnityEngine.Debug;
using RiptideClient = Riptide.Client;

namespace Web.Sync
{
    public class SyncConnection : ISyncConnection
    {
        private readonly RiptideClient _client;
        private readonly CancellationTokenSource _cancellation;

        private Message _movementCommand;

        private SyncConnection(RiptideClient client)
        {
            _client = client;
            _cancellation = new CancellationTokenSource();
        }

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

            var client = new RiptideClient();

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
            _ = connection.RunPublishingLoop(config.SyncLoopInterval);

            return connection;
        }

        public void SendMovement(Vector2 vector2)
        {
            var message = Message
                .Create(MessageSendMode.Reliable, MessageMark.Movement)
                .AddFloat(vector2.x)
                .AddFloat(vector2.y);

            Interlocked.Exchange(ref _movementCommand, message);
        }

        public void Dispose()
        {
            _cancellation.Cancel();
            _client.Disconnect();
        }

        private async Task RunPublishingLoop(TimeSpan interval)
        {
            var stopwatch = new Stopwatch();
            while (!_cancellation.IsCancellationRequested)
            {
                stopwatch.Restart();
                var timestamp = Time.time;

                var movementCommand = Interlocked.Exchange(ref _movementCommand, null);
                if (movementCommand != null)
                {
                    _client.Send(movementCommand.AddFloat(timestamp));
                }

                _client.Update();
                await Task.Delay((interval - stopwatch.Elapsed).AtLeast(TimeSpan.Zero), _cancellation.Token);
            }
        }

        private static Message ConnectToSessionMessage(int sessionId, string playerId, [CanBeNull] string sourceOfTruthKey = null)
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

    internal static class RiptideExtensions
    {
        public delegate T MapMessage<out T>(MessageMark mark, Message message);

        public static async Task<bool> ConnectSequentially
        (
            this RiptideClient client,
            string url,
            TimeSpan refreshInterval,
            CancellationToken? cancellationToken = null
        )
        {
            client.Connect(url);

            while (client.IsConnecting && cancellationToken?.IsCancellationRequested == false)
            {
                await Task.Delay(refreshInterval);
                client.Update();
            }

            return client.IsConnected;
        }

        public static async Task<T> SendWithResponse<T>
        (
            this RiptideClient client,
            Message message,
            MapMessage<T> map,
            TimeSpan refreshInterval,
            CancellationToken? cancellationToken = null
        )
        {
            var task = new TaskCompletionSource<T>();

            cancellationToken?.Register(() =>
            {
                task.TrySetCanceled();
                client.MessageReceived -= OnMessageReceived;
            });
            client.MessageReceived += OnMessageReceived;

            client.Send(message);
            client.Update();

            while (!task.Task.IsCompleted)
            {
                await Task.Delay(refreshInterval);
                client.Update();
                Debug.Log("Awaiting response");
                Debug.Log("client state:\n" +
                         $"IsConnected={client.IsConnected}\n" +
                         $"IsConnecting={client.IsConnecting}\n" +
                         $"IsPending={client.IsPending}");
            }

            return await task.Task;

            void OnMessageReceived(object _, MessageReceivedEventArgs args)
            {
                task.TrySetResult(map((MessageMark)args.MessageId, args.Message));
                client.MessageReceived -= OnMessageReceived;
            }
        }
    }
}
