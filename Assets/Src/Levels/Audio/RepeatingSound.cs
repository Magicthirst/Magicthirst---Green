using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Levels.Audio
{
    [Serializable]
    public class RepeatingSound
    {
        [SerializeField] private float pitchVariance = 0.1f;
        [SerializeField] private AudioClip[] clips;

        public (AudioClip Clip, float Pitch) GetNextClip()
        {
            return
            (
                Clip: clips[Random.Range(0, clips.Length)],
                Pitch: Random.Range(1f - pitchVariance, 1f + pitchVariance)
            );
        }
    }
}