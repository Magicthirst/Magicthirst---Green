using Web;

namespace Levels.Sync
{
    public static class ConnectionRoleUsage
    {
        public static bool IsReceiving(this ConnectionRole role) => role.HasFlag(ConnectionRole.Guest);

        public static bool IsPublishingInput(this ConnectionRole role) => role.HasFlag(ConnectionRole.Guest);

        public static bool IsPublishingUpdates(this ConnectionRole role) => role.HasFlag(ConnectionRole.Host);
    }
}
