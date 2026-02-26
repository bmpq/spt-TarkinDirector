using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class PersistentAudioClip : PlayableAsset
{
    public AudioClip clip;
    public bool loop = false;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<PersistentAudioBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();

        behaviour.clip = clip;
        behaviour.loop = loop;

        return playable;
    }
}

public class PersistentAudioBehaviour : PlayableBehaviour
{
    public AudioClip clip;
    public bool loop;
}
