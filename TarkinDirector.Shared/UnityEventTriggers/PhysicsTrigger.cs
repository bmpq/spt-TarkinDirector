using System;
using UnityEngine;
using UnityEngine.Events;

namespace tarkin.Director
{
    [RequireComponent(typeof(Collider))]
    public class PhysicsTrigger : MonoBehaviour
    {
        public enum Condition
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
