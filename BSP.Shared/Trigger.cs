using UnityEngine;

namespace tarkin.BSP.Shared
{
    [RequireComponent(typeof(Collider))]
    internal class Trigger : AnimatorAction
    {
        enum Condition
        {
            Enter,
            Exit
        }

        [Space(10)]
        [SerializeField]
        private Condition condition;

        Collider col;

        void Start()
        {
            col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (condition == Condition.Enter)
                Invoke();
        }

        void OnTriggerExit(Collider other)
        {
            if (condition == Condition.Exit)
                Invoke();
        }
    }
}
