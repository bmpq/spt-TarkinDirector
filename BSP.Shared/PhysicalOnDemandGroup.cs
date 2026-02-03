using System.Collections.Generic;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    [ExecuteAlways]
    internal class PhysicalOnDemandGroup : MonoBehaviour
    {
        Rigidbody[] rbs;
        Dictionary<Rigidbody, (Vector3, Quaternion, Vector3)> origTrs;

        void Awake()
        {
        }

        void Init()
        {
            if (rbs != null)
                return;
            rbs = GetComponentsInChildren<Rigidbody>();
            origTrs = new Dictionary<Rigidbody, (Vector3, Quaternion, Vector3)>();

            foreach (var rb in rbs)
            {
                rb.isKinematic = true;
                origTrs[rb] = (rb.transform.localPosition, rb.transform.localRotation, rb.transform.localScale);
            }
        }

        private void OnValidate()
        {
            ResetToKinematic();
        }

        public void Trigger()
        {
            Init();

            foreach (var rb in rbs)
            {
                rb.isKinematic = false;
                rb.WakeUp();
            }
        }

        public void ResetToKinematic()
        {
            if (!Application.isPlaying)
                return;

            Init();

            foreach (var rb in rbs)
            {
                rb.isKinematic = true;
                var origTr = origTrs[rb];
                rb.transform.localPosition = origTr.Item1;
                rb.transform.localRotation = origTr.Item2;
                rb.transform.localScale = origTr.Item3;
            }
        }
    }
}