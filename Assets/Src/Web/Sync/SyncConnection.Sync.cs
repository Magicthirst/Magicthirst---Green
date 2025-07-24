using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Riptide;
using UnityEngine;
using Web.Util;
using Debug = UnityEngine.Debug;

namespace Web.Sync
{
    public partial class SyncConnection
    {
        private Message _movementCommand;

        public void SendMovement(Vector2 vector2)
        {
            var message = Message
                .Create(MessageSendMode.Reliable, MessageMark.Movement)
                .AddFloat(vector2.x)
                .AddFloat(vector2.y);

            Interlocked.Exchange(ref _movementCommand, message);
            Debug.Log($"saved {vector2} for publishing");
        }

        private async Task RunSyncLoop(TimeSpan interval)
        {
            var stopwatch = new Stopwatch();
            Debug.Log("start sync loop");
            while (!_cancellation.IsCancellationRequested)
            {
                stopwatch.Restart();

                var movementCommand = Interlocked.Exchange(ref _movementCommand, null)
                    ?.AddDouble(_syncWatch.Elapsed.TotalSeconds);

                if (movementCommand != null)
                {
                    Debug.Log($"sent {movementCommand}");
                    _client.Send(movementCommand);
                }

                _client.Update();
                await Task.Delay((interval - stopwatch.Elapsed).AtLeast(TimeSpan.Zero), _cancellation.Token);
            }
            Debug.Log("end sync loop");
        }
    }
}