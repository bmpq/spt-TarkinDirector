using Comfort.Common;
using System.Collections.Generic;
using tarkin.BSP.Shared;
using UnityEngine;

namespace tarkin.BSP.Bep
{
    public class EFTPersistentAudioSourceHandler
    {
        public BetterAudio.AudioSourceGroupType soundGroup = BetterAudio.AudioSourceGroupType.Environment;

        private Dictionary<PersistentAudioSource, BetterSource> translation = new Dictionary<PersistentAudioSource, BetterSource>();

        public EFTPersistentAudioSourceHandler()
        {
            PersistentAudioSource.OnPlayRequest += HandlePlayRequest;
            PersistentAudioSource.OnStopRequest += HandleStopRequest;
        }

        private void HandlePlayRequest(PersistentAudioSource requester, AudioClip clip, bool loop)
        {
            if (!translation.ContainsKey(requester))
            {
                BetterSource source = Singleton<BetterAudio>.Instance.GetSource(BetterAudio.AudioSourceGroupType.Environment, true);
                source.SetMixerGroup(MonoBehaviourSingleton<BetterAudio>.Instance.VehicleOutMixer);
                source.StartTrackingPosition(requester.transform, default(Vector3));
                source.Loop = loop;

                translation[requester] = source;
            }

            translation[requester].Play(clip, null, 1f, 1f, false, false);
        }

        private void HandleStopRequest(PersistentAudioSource requester)
        {
            if (!translation.ContainsKey(requester))
                return;

            translation[requester].Release();
            translation.Remove(requester);
        }
    }
}
