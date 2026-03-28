using UnityEngine;
using UnityEngine.Events;

namespace tarkin.Director
{
    [RequireComponent(typeof(Collider))]
    internal class PhysicsTrigger : MonoBehaviour
    {
        enum Condition
        {
            Enter,
            Exit
        }

        [Space(10)]
        [SerializeField]
        private Condition condition;

        [SerializeField]
        private UnityEvent unityEvent;

        void OnValidate()
        {
            GetComponent<Collider>().isTrigger = true;
            gameObject.layer = 13;
        }

        void OnTriggerEnter(Collider other)
        {
            if (condition == Condition.Enter)
                unityEvent.Invoke();
        }

        void OnTriggerExit(Collider other)
        {
            if (condition == Condition.Exit)
                unityEvent.Invoke();
        }
    }
}
