using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Shared
{
    public static partial class Settings
    {
        [Serializable]
        public class Audio
        {
            public event Action<float> MasterVolumeDbChanged;
            public event Action<float> MasterVolume01Changed;

            private const string MasterVolumeKey = "SETTINGS/VIDEO/MASTER_VOLUME";

            [SerializeField] private AudioMixer masterVolume;

            public Audio()
            {
                MasterVolumeDbChanged += db => MasterVolume01Changed?.Invoke(To01(db));
            }

            public void Init()
            {
                if (!PlayerPrefs.HasKey(MasterVolumeKey))
                {
                    MasterVolume01 = 0.5f;
                }
            }

            public float MasterVolumeDb
            {
                get => PlayerPrefs.GetFloat(MasterVolumeKey);
                set
                {
                    PlayerPrefs.SetFloat(MasterVolumeKey, value);
                    PlayerPrefs.Save();
                    masterVolume.SetFloat("Volume", value);
                    MasterVolumeDbChanged?.Invoke(value);
                }
            }

            public float MasterVolume01
            {
                get => To01(MasterVolumeDb);
                set => MasterVolumeDb = From01(value);
            }

            private static float To01(float db) => Mathf.Pow(10, db / 20f);

            private static float From01(float value) => Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        }
    }
}