using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace tarkin.Director
{
    [TrackColor(0.8f, 0.2f, 0.8f)]
    [TrackClipType(typeof(PersistentAudioClip))]
    [TrackBindingType(typeof(PersistentAudioSource))]
    public class PersistentAudioTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<PersistentAudioMixer>.Create(graph, inputCount);
        }
    }
}