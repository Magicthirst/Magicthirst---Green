using System;
using Levels.Core;
using UnityEngine;

namespace Levels.Abilities.PushingShotgun
{
    [Serializable]
    public class ShotgunConfig : IConfig
    {
        [field: SerializeField] public int Damage { get; set; }
        [field: SerializeField] public float Velocity { get; set; }
        [field: SerializeField] public float Duration { get; set; }
        [field: SerializeField] public float CircleRadius { get; set; }
        [field: SerializeField] public float CircleCenterOffset { get; set; }
    }
}