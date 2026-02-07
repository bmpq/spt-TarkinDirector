using System;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class SceneSettings : MonoBehaviour
    {
        [SerializeField] private bool overrideShadowDistance;
        [SerializeField] private float shadowDistance = 40f;

        private void LateUpdate()
        {
            if (overrideShadowDistance)
            {
                QualitySettings.shadowDistance = shadowDistance;
            }
        }
    }
}
