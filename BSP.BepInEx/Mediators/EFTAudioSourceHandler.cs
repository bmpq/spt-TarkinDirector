using Comfort.Common;
using System.Collections.Generic;
using tarkin.BSP.Shared;
using UnityEngine;

namespace tarkin.BSP.Bep.Mediators
{
    public class EFTPersistentAudioSourceHandler
    {
        private Dictionary<PersistentAudioSource, BetterSource> instances = new Dictionary<PersistentAudioSource, BetterSource>();

        public EFTPersistentAudioSourceHandler()
        {
            PersistentAudioSource.OnPlayRequest += HandlePlayRequest;
            PersistentAudioSource.OnStopRequest += HandleStopRequest;
        }

        private void HandlePlayRequest(PersistentAudioSource requester, AudioClip clip, bool loop)
        {
            if (!instances.ContainsKey(requester))
            {
                BetterSource source = Singleton<BetterAudio>.Instance.GetSource(BetterAudio.AudioSourceGroupType.Character, true);
                source.SetMixerGroup(MonoBehaviourSingleton<BetterAudio>.Instance.ObservedPlayerMovementMixer);
                source.StartTrackingPosition(requester.transform, default);
                source.Loop = loop;

                instances[requester] = source;
            }

            instances[requester].Play(clip, null, 1f, 1f, false, false);
        }

        private void HandleStopRequest(PersistentAudioSource requester)
        {
            if (!instances.ContainsKey(requester))
                return;

            instances[requester].Release();
            instances.Remove(requester);
        }
    }
}
