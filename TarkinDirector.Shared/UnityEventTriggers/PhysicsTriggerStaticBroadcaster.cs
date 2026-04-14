using System;
using UnityEngine;

namespace tarkin.Director.Shared
{
    public class PhysicsTriggerStaticBroadcaster : MonoBehaviour
    {
        public static event Action<PhysicsTriggerStaticBroadcaster, Collider> OnEnter;
        public static event Action<PhysicsTriggerStaticBroadcaster, Collider> OnExit;

        void OnTriggerEnter(Collider other)
        {
            OnEnter?.Invoke(this, other);
        }

        void OnTriggerExit(Collider other)
        {
            OnExit?.Invoke(this, other);
        }
    }
}
