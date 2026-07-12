using System.Collections;
using Levels.Directorship;
using UnityEngine;

namespace Levels.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class PlaySoundOnActivity : LevelBehaviour
    {
        protected override LevelActivityMask _LifecycleMask => (LevelActivityMask)mask;

        [SerializeField] private EditorLevelActivityMask mask;
        [SerializeField] private RepeatingSound sound;
        [SerializeField] private float startOffset;

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        protected override void DidEnabled()
        {
            StartCoroutine(PlaySound());
        }

        private IEnumerator PlaySound()
        {
            yield return new WaitForSeconds(startOffset);
            var (clip, pitch) = sound.GetNextClip();
            _source.PlayOneShot(clip);
            _source.pitch = pitch;
        }
    }
}