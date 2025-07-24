using System;

namespace Web
{
    public record ClientConfig
    (
        string GatewayUrl,
        TimeSpan TokenRefreshInterval,
        TimeSpan SyncLoopInterval,
        TimeSpan ConnectionTimeout
    );
}
