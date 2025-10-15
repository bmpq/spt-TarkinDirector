using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class SceneSettings : MonoBehaviour
    {
        [SerializeField] private SimulationMode physicsMode;

        void OnEnable()
        {
            Physics.simulationMode = physicsMode;
        }
    }
}
