using System;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class TripwireImposter : MonoBehaviour
    {
        public static event Action<TripwireImposter> OnRequestSpawn;
        public static event Action<TripwireImposter> OnRequestRemove;

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
    }
}
