using System;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    [RequireComponent(typeof(Collider))]
    public class MyMovingPlatform : MonoBehaviour
    {
        public static event Action<MyMovingPlatform, Collider> ActionOnTriggerEnter;
        public static event Action<MyMovingPlatform, Collider> ActionOnTriggerExit;

        public static event Action<MyMovingPlatform, Vector3> ActionLateUpdatePositionDelta;

        public static event Action<MyMovingPlatform> ActionOnDestroy;

        Vector3 prevPos;

        void Start()
        {
            prevPos = transform.position;

            GetComponent<Collider>().isTrigger = true;
        }

        void LateUpdate()
        {
            ActionLateUpdatePositionDelta?.Invoke(this, transform.position - prevPos);

            prevPos = transform.position;
        }

        void OnTriggerEnter(Collider col)
        {
            ActionOnTriggerEnter?.Invoke(this, col);
        }

        void OnTriggerExit(Collider col)
        {
            ActionOnTriggerExit?.Invoke(this, col);
        }

        void OnDisable()
        {
            ActionOnDestroy?.Invoke(this);
        }
    }
}
