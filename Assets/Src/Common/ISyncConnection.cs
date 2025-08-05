using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    using IdsPlayers = Dictionary<int, PlayerState>;

    public delegate void MovementCommand(Vector2 position, Vector2 vector, double elapsedSeconds);

    public interface ISyncConnection : IDisposable, IReinitSource
    {
        public int SelfId { get; }

        public IProducer Self { get; }

        public IConsumer GetForIndividual(int playerId);
    }

    public interface IConsumer
    {
        public event MovementCommand MovementCommanded;
    }

    public interface IProducer
    {
        public void SendMovement(Vector2 position, Vector2 vector);

        public void Exit();
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
