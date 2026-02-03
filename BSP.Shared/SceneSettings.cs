using System;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class SceneSettings : MonoBehaviour
    {
        [SerializeField] private SimulationMode physicsMode;

        [SerializeField] private bool overrideShadowDistance;
        [SerializeField] private float shadowDistance = 40f;

        void OnEnable()
        {
            Physics.simulationMode = physicsMode;
        }

        private void LateUpdate()
        {
            if (overrideShadowDistance)
            {
                QualitySettings.shadowDistance = shadowDistance;
            }
        }
    }
}
