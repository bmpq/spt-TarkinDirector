using System;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public struct EffectRequest
    {
        public string Name;
        public Vector3 Position;
        public Vector3 Normal;
        public float Magnitude;
        public float Volume;
        public bool DrawDecal;
    }

    public class EffectEmitter : MonoBehaviour
    {
        public static event Action<EffectRequest> OnRequest;

        public string Name = "grenade_new";

        public float Magnitude = 1f;

        [Range(0f, 1f)]
        public float Volume = 0f;
        public bool DrawDecal = false;

        public float Interval = 0.2f;

        private float _timer;

        void OnEnable()
        {
            RequestEffect();
        }

        void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= Interval)
            {
                _timer -= Interval;
                RequestEffect();
            }
        }

        public void RequestEffect()
        {
            EffectRequest request = new EffectRequest
            {
                Name = this.Name,
                Position = transform.position,
                Normal = transform.forward,
                Magnitude = this.Magnitude,
                Volume = this.Volume,
                DrawDecal = this.DrawDecal
            };

            OnRequest?.Invoke(request);
        }
    }
}