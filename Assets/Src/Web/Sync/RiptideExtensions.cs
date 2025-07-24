using System;
using System.Threading;
using System.Threading.Tasks;
using Riptide;
using UnityEngine;

namespace Web.Sync
{
    internal static class RiptideExtensions
    {
        public delegate T MapMessage<out T>(MessageMark mark, Message message);

        public static async Task<bool> ConnectSequentially
        (
            this Client client,
            string url,
            TimeSpan refreshInterval,
            CancellationToken? cancellationToken = null
        )
        {
            client.Connect(url, useMessageHandlers: false);

            while (client.IsConnecting && cancellationToken?.IsCancellationRequested == false)
            {
                await Task.Delay(refreshInterval);
                client.Update();
            }

            return client.IsConnected;
        }

        public static async Task<T> SendWithResponse<T>
        (
            this Client client,
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