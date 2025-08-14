using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Riptide;
using UnityEngine;
using Web.Util;
using Debug = UnityEngine.Debug;

namespace Web.Sync
{
    public partial class SyncConnection
    {
        private async Task RunSyncLoop(TimeSpan interval)
        {
            _syncWatch.Start();
            var stopwatch = new Stopwatch();
            Debug.Log("start sync loop");
            while (!_cancellation.IsCancellationRequested)
            {
                stopwatch.Restart();

                var movementCommand = Interlocked.Exchange(ref _self.MovementCommand, null)
                    ?.AddDouble(_syncWatch.Elapsed.TotalSeconds);

                if (movementCommand != null)
                {
                    _client.Send(movementCommand);
                    Debug.Log($"sent {nameof(movementCommand)} at {_syncWatch.Elapsed.TotalSeconds}");
                }

                _client.Update();
                await Task.Delay((interval - stopwatch.Elapsed).AtLeast(TimeSpan.Zero), _cancellation.Token);
            }
            Debug.Log("end sync loop");
        }

        internal class Producer : IProducer
        {
            internal Message MovementCommand;

            internal event Action Exited;

            public void SendMovement(Vector2 position, Vector2 vector)
            {
                var message = Message
                    .Create(MessageSendMode.Reliable, MessageMark.Movement)
                    .AddVector2(position)
                    .AddVector2(vector);

                Interlocked.Exchange(ref MovementCommand, message);
                Debug.Log($"saved {(position, vector)} for publishing");
            }

            public void Exit() => Exited?.Invoke();
        }
    }
}