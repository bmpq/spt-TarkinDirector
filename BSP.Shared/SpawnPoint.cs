using System;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class SpawnPoint : MonoBehaviour
    {
        public static event Action<SpawnPoint> OnEnableEvent;

        void OnEnable()
        {
            OnEnableEvent?.Invoke(this);
        }
    }
}
