using UnityEngine;

namespace Web.Sync
{
    [CreateAssetMenu(fileName = "SyncConfig", menuName = "Sync/Sync Config", order = 1)]
    public class SyncConfig : ScriptableObject
    {
        public string hostAddress;
        [Range(1f / 60, float.PositiveInfinity)] public float clientUpdateDt;
        [Range(1f / 60, float.PositiveInfinity)] public float publishUpdateDt;
    }
}
