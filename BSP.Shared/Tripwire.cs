#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class Tripwire : MonoBehaviour
    {
        public static event Action<Tripwire> OnRequestSpawn;
        public static event Action<Tripwire> OnRequestRemove;

        public Transform end;

        public Vector3 PosFrom
        {
            get => transform.position;
            set => transform.position = value;
        }
        public Vector3 PosTo
        {
            get  {
                if (end == null)
                    Reset();
                return end.position;
            }
            set => end.position = value;
        }
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
        [DrawGizmo(GizmoType.Pickable | GizmoType.NonSelected)]
        private void OnDrawGizmos()
        {
            if (end == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;

            float height = 0.2f;

            Gizmos.DrawLine(PosFrom, PosFrom + new Vector3(0, height, 0));
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.DrawDottedLine(PosFrom, PosTo, 1f);
            Gizmos.DrawLine(PosFrom + new Vector3(0, height, 0), PosTo + new Vector3(0, height, 0));
            Gizmos.DrawLine(PosTo, PosTo + new Vector3(0, height, 0));

            float crossSize = 0.05f;

            Gizmos.DrawLine(PosFrom - new Vector3(crossSize / 2f, 0, 0), PosFrom + new Vector3(crossSize / 2f, 0, 0));
            Gizmos.DrawLine(PosFrom - new Vector3(0, 0, crossSize / 2f), PosFrom + new Vector3(0, 0, crossSize / 2f));

            Gizmos.DrawLine(PosTo - new Vector3(crossSize / 2f, 0, 0), PosTo + new Vector3(crossSize / 2f, 0, 0));
            Gizmos.DrawLine(PosTo - new Vector3(0, 0, crossSize / 2f), PosTo + new Vector3(0, 0, crossSize / 2f));

            Gizmos.DrawWireSphere(PosFrom, 0.02f);
            Gizmos.DrawWireSphere(PosTo, 0.02f);

            Vector3 dirTo = (PosTo - PosFrom).normalized;
            Vector3 grenadePos = PosTo + new Vector3(0, height - 0.05f, 0) - dirTo * 0.05f;
            Gizmos.DrawCube(grenadePos, new Vector3(0.04f, 0.05f, 0.04f));
        }
#endif
    }
}