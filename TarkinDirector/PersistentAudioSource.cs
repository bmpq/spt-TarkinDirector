using System;
using UnityEngine;

#if EFT_RUNTIME
using Comfort.Common;
#endif

namespace tarkin.Director
{
    public class PersistentAudioSource : MonoBehaviour
    {
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
#elif EFT_RUNTIME
        BetterSource eftSource;
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

#if EFT_RUNTIME
            if (eftSource == null)
            {
                eftSource = Singleton<BetterAudio>.Instance.GetSource(BetterAudio.AudioSourceGroupType.Character, true);
                eftSource.SetMixerGroup(MonoBehaviourSingleton<BetterAudio>.Instance.ObservedPlayerMovementMixer);
                eftSource.StartTrackingPosition(transform, default);
                eftSource.Loop = loop;
            }
            eftSource.Play(clip, null, balance: 1f, volume: 1f, forceStereo: false, oneShot: false);
#elif UNITY_EDITOR
            audioSource.clip = clip;
            audioSource.Play();
#endif
        }

        public void Stop()
        {
#if EFT_RUNTIME
            eftSource.Release();
            eftSource = null;
#elif UNITY_EDITOR
            audioSource.Stop();
#endif
        }
    }
}
