using System;
using Riptide;
using UnityEngine;

namespace Web.Sync
{
    public struct PlayerUpdate
    {
        public uint Mask =>
            (YAngleDegrees != null ? 1u << 0 : 0u) |
            (Position != null ? 1u << 1 : 0u) |
            (Velocity != null ? 1u << 2 : 0u);

        public float? YAngleDegrees;
        public (float x, float y, float z)? Position;
        public (float x, float y, float z)? Velocity;

        public bool IsEmpty() => Mask != 0u;

        public void Clear()
        {
            YAngleDegrees = null;
            Velocity = null;
        }
    }

    public static class Messages
    {
        public static Message AddPlayerUpdate(this Message message, PlayerUpdate update)
        {
            message.AddFloat(Time.fixedTime);
            message.AddUInt(update.Mask);

            if (update.YAngleDegrees.HasValue)
            {
                message.AddFloat(update.YAngleDegrees.Value);
            }
            if (update.Position.HasValue)
            {
                message.AddFloat(update.Position.Value.x);
                message.AddFloat(update.Position.Value.y);
            }
            if (update.Velocity.HasValue)
            {
                message.AddFloat(update.Velocity.Value.x);
                message.AddFloat(update.Velocity.Value.y);
            }

            return message;
        }

        public static PlayerUpdate GetPlayerUpdate(this Message message)
        {
            var mask = message.GetInt();
            var update = new PlayerUpdate();

            if ((mask & 1u << 0) != 0)
            {
                update.YAngleDegrees = message.GetFloat();
            }
            if ((mask & 1u << 1) != 0)
            {
                update.Position = (message.GetFloat(), message.GetFloat(), message.GetFloat());
            }
            if ((mask & 1u << 2) != 0)
            {
                update.Velocity = (message.GetFloat(), message.GetFloat(), message.GetFloat());
            }

            return update;
        }

        public static Timed<T> GetTimed<T>(this Message message, Func<Message, T> getValue) => new()
        {
            SentAt = message.GetFloat(),
            Value = getValue(message)
        };
    }

    public struct Timed<T>
    {
        public float SentAt;
        public T Value;
    }
}