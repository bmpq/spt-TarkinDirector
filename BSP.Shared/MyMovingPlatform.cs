using System;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    [RequireComponent(typeof(Collider))]
    public class MyMovingPlatform : MonoBehaviour
    {
        public event Action<Collider> ActionOnTriggerEnter;
        public event Action<Collider> ActionOnTriggerExit;

        public event Action<Vector3> ActionLateUpdatePositionDelta;

        public event Action ActionOnDestroy;

        Vector3 prevPos;

        void Start()
        {
            prevPos = transform.position;

            GetComponent<Collider>().isTrigger = true;
        }

        void LateUpdate()
        {
            ActionLateUpdatePositionDelta?.Invoke(transform.position - prevPos);

            prevPos = transform.position;
        }

        void OnTriggerEnter(Collider col)
        {
            ActionOnTriggerEnter?.Invoke(col);
        }

        void OnTriggerExit(Collider col)
        {
            ActionOnTriggerExit?.Invoke(col);
        }

        void OnDestroy()
        {
            ActionOnDestroy?.Invoke();

            ActionOnTriggerEnter = null;
            ActionOnTriggerExit = null;
            ActionLateUpdatePositionDelta = null;
            ActionOnDestroy = null;
        }
    }
}
