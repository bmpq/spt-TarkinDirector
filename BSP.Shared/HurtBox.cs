using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    [RequireComponent(typeof(Collider))]
    public class HurtBox : MonoBehaviour
    {
        public static event Action<HurtBox, Collider> OnHit;

        public EDamageType DamageType;
        public float DamageAmount = 10f;

        [SerializeField] private float TickInterval = 1f;
        [SerializeField] private bool DamageOnEnter = true;

        private Dictionary<Collider, Coroutine> _activeCoroutines = new Dictionary<Collider, Coroutine>();

        void Reset()
        {
            var c = GetComponent<Collider>();
            if (c) c.isTrigger = true;
            gameObject.layer = 13;
        }

        void OnTriggerEnter(Collider other)
        {
            if (_activeCoroutines.ContainsKey(other)) return;

            if (DamageOnEnter)
                OnHit?.Invoke(this, other);

            var coroutine = StartCoroutine(TickDamageCoroutine(other));
            _activeCoroutines.Add(other, coroutine);
        }

        IEnumerator TickDamageCoroutine(Collider other)
        {
            if (TickInterval <= 0f) yield break;

            while (other != null && other.gameObject != null && other.enabled && gameObject.activeInHierarchy)
            {
                yield return new WaitForSeconds(TickInterval);

                if (!_activeCoroutines.ContainsKey(other)) yield break;

                if (other == null) break;

                OnHit?.Invoke(this, other);
            }

            if (_activeCoroutines.ContainsKey(other))
                _activeCoroutines.Remove(other);
        }

        void OnTriggerExit(Collider other)
        {
            if (_activeCoroutines.TryGetValue(other, out var c))
            {
                StopCoroutine(c);
                _activeCoroutines.Remove(other);
            }
        }

        void OnDisable()
        {
            foreach (var kv in _activeCoroutines)
                if (kv.Value != null) StopCoroutine(kv.Value);

            _activeCoroutines.Clear();
        }
    }

    [Flags]
    public enum EDamageType
    {
        Undefined = 1,
        Fall = 2,
        Explosion = 4,
        Barbed = 8,
        Flame = 16,
        GrenadeFragment = 32,
        Impact = 64,
        Existence = 128,
        Medicine = 256,
        Bullet = 512,
        Melee = 1024,
        Landmine = 2048,
        Sniper = 4096,
        Blunt = 8192,
        LightBleeding = 16384,
        HeavyBleeding = 32768,
        Dehydration = 65536,
        Exhaustion = 131072,
        RadExposure = 262144,
        Stimulator = 524288,
        Poison = 1048576,
        LethalToxin = 2097152,
        Btr = 4194304,
        Artillery = 8388608,
        Environment = 16777216
    }
}