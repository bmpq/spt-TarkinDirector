using System;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class Tripwire : MonoBehaviour
    {
        public static event Action<Tripwire> OnRequestSpawn;
        public static event Action<Tripwire> OnRequestRemove;

        [SerializeField]
        private Transform end;

        public Vector3 PosFrom => transform.position;
        public Vector3 PosTo => end.position;
        public string GrenadeGuid = "5e340dcdcb6d5863cc5e5efb";

        void OnEnable()
        {
            OnRequestSpawn?.Invoke(this);
        }

        void OnDisable()
        {
            OnRequestRemove?.Invoke(this);
        }

        void Reset()
        {
            if (transform.childCount > 0)
            {
                end = transform.GetChild(0);
            }
            else
            {
                end = new GameObject("end").transform;
                end.SetParent(transform, false);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (end == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;

            float height = 0.2f;

            Gizmos.DrawLine(PosFrom, PosFrom + new Vector3(0, height, 0));
            Gizmos.DrawLine(PosFrom, PosTo);
            Gizmos.DrawLine(PosFrom + new Vector3(0, height, 0), PosTo + new Vector3(0, height, 0));
            Gizmos.DrawLine(PosTo, PosTo + new Vector3(0, height, 0));
            
            Gizmos.DrawSphere(PosFrom, 0.02f);
            Gizmos.DrawSphere(PosTo, 0.02f);
        }
#endif
    }
}
