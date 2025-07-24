using System;
using UnityEngine;
using Web;

namespace Assets
{
    [CreateAssetMenu(menuName = "Create ClientConfigAsset", fileName = "ClientConfigAsset", order = 0)]
    public class ClientConfigAsset : ScriptableObject
    {
        public string gatewayUrl;
        public float tokenRefreshIntervalSeconds;
        public float syncLoopIntervalSeconds;
        public float connectionTimeoutSeconds;

        public ClientConfig ToRecord() => new
        (
            gatewayUrl,
            TimeSpan.FromSeconds(tokenRefreshIntervalSeconds),
            TimeSpan.FromSeconds(syncLoopIntervalSeconds),
            TimeSpan.FromSeconds(connectionTimeoutSeconds)
        );
    }
}
