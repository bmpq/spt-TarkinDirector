using System.Collections.Generic;
using UnityEngine;
using tarkin.BSP.Shared;

namespace tarkin.BSP.Editor
{
    internal class PersistentAudioSourceEditorPreview : MonoBehaviour
    {
        private Dictionary<PersistentAudioSource, AudioSource> translation = new Dictionary<PersistentAudioSource, AudioSource>();

        void OnEnable()
        {
            PersistentAudioSource.OnPlayRequest += HandlePlayRequest;
            PersistentAudioSource.OnStopRequest += HandleStopRequest;
        }

        void OnDisable()
        {
            PersistentAudioSource.OnPlayRequest -= HandlePlayRequest;
            PersistentAudioSource.OnStopRequest -= HandleStopRequest;

            foreach (var kvp in translation)
            {
                Destroy(kvp.Value);
            }
            translation.Clear();
        }

        private void HandlePlayRequest(PersistentAudioSource requester, AudioClip clip, bool loop)
        {
            if (!translation.ContainsKey(requester))
            {
                AudioSource source = requester.gameObject.AddComponent<AudioSource>();

                translation[requester] = source;
            }

            translation[requester].loop = loop;
            translation[requester].clip = clip;
            translation[requester].spatialBlend = 1f;
            translation[requester].Play();
        }

        private void HandleStopRequest(PersistentAudioSource requester)
        {
            if (!translation.ContainsKey(requester))
                return;

            Destroy(translation[requester]);
            translation.Remove(requester);
        }
    }
}