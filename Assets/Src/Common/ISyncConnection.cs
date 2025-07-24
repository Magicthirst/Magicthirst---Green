using System;
using UnityEngine;

namespace Common
{
    public interface ISyncConnection : IDisposable
    {
        void SendMovement(Vector2 vector2);
    }
}
