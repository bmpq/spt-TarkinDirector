using System;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class PersistentAudioSource : MonoBehaviour
    {
        public static event Action<PersistentAudioSource, AudioClip, bool> OnPlayRequest;
        public static event Action<PersistentAudioSource> OnStopRequest;

        [SerializeField] private AudioClip clip; 
        [SerializeField] private bool playOnAwake = false;
        [SerializeField] private bool loop = false;

        void OnEnable()
        {
            if (playOnAwake)
            {
                Play();
            }
        }

        void OnDisable()
        {
            Stop();
        }

        public void Play()
        {
            if (clip == null)
                return;

            OnPlayRequest?.Invoke(this, clip, loop);
        }

        public void Stop()
        {
            OnStopRequest?.Invoke(this);
        }
    }
}
