using System;
using System.Collections.Generic;
using Common;
using Riptide;
using UnityEngine;

namespace Web.Sync
{
    public partial class SyncConnection
    {
        event Action<Dictionary<int, PlayerState>> IReinitSource.Reinited
        {
            add
            {
                Reinited += value;
                value(_lastReceivedReinit);
            }
            remove => Reinited -= value;
        }

        private event Action<Dictionary<int, PlayerState>> Reinited;

        private Dictionary<int, PlayerState> _lastReceivedReinit = new();

        private void ReceiveReinit(Message message)
        {
            var count = message.GetInt();
            var players = new Dictionary<int, PlayerState>();
            players.EnsureCapacity(count);

            for (var i = 0; i < count; i++)
            {
                var playerId = message.GetInt();
                var state = new PlayerState
                (
                    Position: message.GetVector2(),
                    Vector: message.GetVector2()
                );

                players.Add(playerId, state);
            }

            Debug.Log($"received reinit: {players}, |P| = {players.Count}");
            Reinited?.Invoke(players);
            _lastReceivedReinit = players;
        }

        private void ReceiveMovement(Message message)
        {
            var position = message.GetVector2();
            var vector = message.GetVector2();
            var timestamp = message.GetDouble();
            var sender = message.GetInt();

            if (_consumers.TryGetValue(sender, out var consumer))
            {
                consumer.CommandMovement(position, vector, timestamp);
            }

            Debug.Log($"received movement: {(position, vector, timestamp)}");
        }

        internal class Consumer : IConsumer
        {
            public event MovementCommand MovementCommanded;

            internal void CommandMovement(Vector2 position, Vector2 vector, double timestampSeconds) =>
                MovementCommanded?.Invoke(position, vector, timestampSeconds);
        }
    }
}