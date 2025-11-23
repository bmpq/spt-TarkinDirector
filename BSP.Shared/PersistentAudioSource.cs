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

#if UNITY_EDITOR
        AudioSource audioSource
        {
            get
            {
                var source = GetComponent<AudioSource>();
                if (source == null)
                    return gameObject.AddComponent<AudioSource>();
                return source;
            }
        }
#endif

        public void SetClip(AudioClip newClip, bool newLoopState)
        {
            clip = newClip;
            loop = newLoopState;
        }

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

#if UNITY_EDITOR
            audioSource.clip = clip;
            audioSource.Play();
#endif
        }

        public void Stop()
        {
            OnStopRequest?.Invoke(this);

#if UNITY_EDITOR
            audioSource.Stop();
#endif
        }
    }
}
