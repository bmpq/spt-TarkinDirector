using UnityEngine;

namespace tarkin.Director.UnityPhysics
{
    public class PhysicsSettings : MonoBehaviour
    {
        [SerializeField] private SimulationMode physicsMode = SimulationMode.Script;

        [SerializeField] float fixedDeltaTime = 0.01666667f;

        [SerializeField] int defaultSolverIterations = 3;
        [SerializeField] int defaultSolverVelocityIterations = 1;

        void OnEnable()
        {
            Physics.simulationMode = physicsMode;

            Time.fixedDeltaTime = fixedDeltaTime;

            Physics.defaultSolverIterations = defaultSolverIterations;
            Physics.defaultSolverVelocityIterations = defaultSolverVelocityIterations;
        }
    }
}
