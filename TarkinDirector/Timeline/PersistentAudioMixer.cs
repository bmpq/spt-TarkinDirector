using UnityEngine;
using UnityEngine.Playables;
using tarkin.Director;

public class PersistentAudioMixer : PlayableBehaviour
{
    private AudioClip _currentClip;
    private bool _isPlaying;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        PersistentAudioSource audioSource = playerData as PersistentAudioSource;

        if (audioSource == null) return;

        int inputCount = playable.GetInputCount();
        float totalWeight = 0f;

        AudioClip targetClip = null;
        bool targetLoop = false;

        float maxWeight = 0f;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);

            if (inputWeight > maxWeight)
            {
                var inputPlayable = (ScriptPlayable<PersistentAudioBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();

                maxWeight = inputWeight;
                targetClip = input.clip;
                targetLoop = input.loop;
            }
            totalWeight += inputWeight;
        }

        if (targetClip != null && (targetClip != _currentClip || !_isPlaying))
        {
            if (_isPlaying && targetClip != _currentClip)
            {
                audioSource.Stop();
            }

            audioSource.SetClip(targetClip, targetLoop);

            audioSource.Play();

            _currentClip = targetClip;
            _isPlaying = true;
        }
        else if (totalWeight <= 0.001f && _isPlaying)
        {
            audioSource.Stop();
            _isPlaying = false;
            _currentClip = null;
        }
    }

    public override void OnGraphStop(Playable playable)
    {
        if (_isPlaying)
        {
            _isPlaying = false;
            _currentClip = null;
        }
    }
}
