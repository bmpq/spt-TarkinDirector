using UnityEngine;

namespace tarkin.Director.UnityPhysics
{
    public class PhysicsSettings : MonoBehaviour
    {
        [SerializeField] private SimulationMode physicsMode = SimulationMode.Script;

        [Range(1, 120)]
        [SerializeField] private int ticksPerSecond = 60;

        [SerializeField] int defaultSolverIterations = 3;
        [SerializeField] int defaultSolverVelocityIterations = 1;

        void OnEnable()
        {
            Physics.simulationMode = physicsMode;

            Time.fixedDeltaTime = 1f / (float)ticksPerSecond;

            Physics.defaultSolverIterations = defaultSolverIterations;
            Physics.defaultSolverVelocityIterations = defaultSolverVelocityIterations;
        }
    }
}
