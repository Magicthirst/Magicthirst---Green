using System;

namespace Web
{
    public record ClientConfig(string GatewayUrl, TimeSpan RefreshInterval);
}
