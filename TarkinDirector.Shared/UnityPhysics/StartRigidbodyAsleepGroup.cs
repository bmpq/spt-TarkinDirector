using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tarkin.Director.UnityPhysics
{
    public class StartRigidbodyAsleepGroup : MonoBehaviour
    {
        int counter = 1;

        void Awake()
        {
        }

        private void FixedUpdate()
        {
            if (counter < 0)
            {
                Destroy(this);
            }
            counter--;

            foreach (var rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.Sleep();
            }
        }
    }
}
