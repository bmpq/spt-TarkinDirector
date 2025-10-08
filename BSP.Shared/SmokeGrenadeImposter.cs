using System;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class SmokeGrenadeImposter : MonoBehaviour
    {
        public static event Action<SmokeGrenadeImposter> OnRequestStart;
        public static event Action<SmokeGrenadeImposter> OnRequestStop;

        public float EmitTime = 90f;

        [SerializeField] private float delay = 0f;

        private float _time;

        bool isEmitting;

        void OnEnable()
        {
            _time = 0f;
            isEmitting = false;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            if (!isEmitting && _time > delay)
            {
                OnRequestStart?.Invoke(this);
                isEmitting = true;
            }

            if (_time - delay > EmitTime)
            {
                this.enabled = false;
            }
        }

        void OnDisable()
        {
            OnRequestStop?.Invoke(this);
        }
    }
}