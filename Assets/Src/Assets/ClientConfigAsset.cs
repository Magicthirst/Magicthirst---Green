using System;
using UnityEngine;
using Web;

namespace Assets
{
    [CreateAssetMenu(menuName = "Create ClientConfigAsset", fileName = "ClientConfigAsset", order = 0)]
    public class ClientConfigAsset : ScriptableObject
    {
        public string gatewayUrl;
        public float refreshIntervalSeconds;

        private TimeSpan _RefreshInterval => TimeSpan.FromSeconds(refreshIntervalSeconds);

        public ClientConfig ToRecord() => new(gatewayUrl, _RefreshInterval);
    }
}
