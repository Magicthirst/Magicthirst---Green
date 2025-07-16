using Common;

namespace Model
{
    public class ConnectionRoleState : IAssignConnectionRole
    {
        private ConnectionRole _connectionRole = ConnectionRole.Offline;

        public bool IsReceiving() => _connectionRole.HasFlag(ConnectionRole.Guest);

        public bool IsPublishingInput() => _connectionRole.HasFlag(ConnectionRole.Guest);

        public bool IsPublishingUpdates() => _connectionRole.HasFlag(ConnectionRole.Host);

        public void Offline() => _connectionRole = ConnectionRole.Offline;

        public void Host() => _connectionRole = ConnectionRole.Host;

        public void Guest() => _connectionRole = ConnectionRole.Guest;
    }
}
