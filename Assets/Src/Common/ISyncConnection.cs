using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    using IdsPlayers = Dictionary<int, PlayerState>;

    public delegate void MovementCommand(Vector2 position, Vector2 vector, double timestampSeconds);

    public interface ISyncConnection : IDisposable, IReinitSource
    {
        IProducer Self { get; }

        IConsumer GetForIndividual(int playerId);
    }

    public interface IConsumer
    {
        event MovementCommand MovementCommanded;
    }

    public interface IProducer
    {
        void SendMovement(Vector2 position, Vector2 vector);

        void Exit();
    }

    public interface IReinitSource
    {
        event Action<IdsPlayers> Reinited;
    }

    public sealed record PlayerState(Vector2 Position, Vector2 Vector)
    {
        public override string ToString() => $"{nameof(PlayerState)}(Position={Position}, Vector={Vector})";
    }
}
