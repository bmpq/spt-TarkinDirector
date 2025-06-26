using UnityEngine;

namespace tarkin.BSP.Shared
{
    [RequireComponent(typeof(Collider))]
    internal class Trigger : AnimatorAction
    {
        Collider col;

        void Start()
        {
            col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            Invoke();
        }
    }
}
