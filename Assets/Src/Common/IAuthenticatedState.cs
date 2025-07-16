using System;

namespace Common
{
    public interface IAuthenticatedState
    {
        public event Action<bool> IsAuthenticatedChanged;

        public bool IsAuthenticated { get; }
    }
}
