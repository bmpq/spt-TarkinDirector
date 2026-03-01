using System;
using UnityEngine;

#if EFT_RUNTIME
using Systems.Effects;
using Comfort.Common;
#endif

namespace tarkin.Director
{
    public class SmokeGrenade : MonoBehaviour
    {
        [SerializeField] private float delay = 0f;
        [SerializeField] private float emitTime = 90f;

        private float _time;

        bool isEmitting;

        const string emissionEffect = "weapon_m18_world";

        void OnEnable()
        {
            StopEmitting();

            _time = 0f;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            if (!isEmitting && _time > delay)
            {
                StartEmitting();
            }

            if (_time - delay > emitTime)
            {
                this.enabled = false;
            }
        }

        void OnDisable()
        {
            StopEmitting();
        }

#if EFT_RUNTIME
        GrenadeEmission grenadeEmission;
#endif
        void StartEmitting()
        {
            isEmitting = true;

#if EFT_RUNTIME
            grenadeEmission = Singleton<Effects>.Instance.GetEmissionEffect(emissionEffect);

            grenadeEmission.AttachTo(transform, offset: Vector3.zero);
            grenadeEmission.SetFillParams(timePastSinceStart: 0f, emitTime);
            grenadeEmission.StartEmission(prewarm: 0f);
#endif
        }

        void StopEmitting()
        {
            isEmitting = false;

#if EFT_RUNTIME
            if (grenadeEmission != null)
                grenadeEmission.StopEmission(null);
#endif
        }
    }
}