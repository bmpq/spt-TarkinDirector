using System;
using System.Linq;
using UnityEngine;

#if EFT_RUNTIME
using Systems.Effects;
using Comfort.Common;
#endif

namespace tarkin.Director
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

        void RequestEffect()
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

#if EFT_RUNTIME
            if (!Singleton<Effects>.Instantiated)
                return;
            Effects.Effect effect = Singleton<Effects>.Instance.EffectsArray.Where(e => e.Name == request.Name).FirstOrDefault();
            if (effect == null)
                return;
            Singleton<Effects>.Instance.AddEffectEmit(effect, request.Position, request.Normal, null, request.DrawDecal, request.Volume);
#endif
        }
    }
}