using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Riptide;
using UnityEngine;
using Web.Util;

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
    }
}