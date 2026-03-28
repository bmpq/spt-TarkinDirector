using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace tarkin.Director
{
    public class MonoBehaviourTriggerReflectionStaticMethod : MonoBehaviour
    {
        [SerializeField] private float onStartDelaySeconds = 0f;

        [SerializeField]
        [Tooltip("Format: 'Namespace.Class.Method' OR 'Assembly::Namespace.Class.Method'")]
        private string methodOnStart;

        [SerializeField]
        [Tooltip("Format: 'Namespace.Class.Method' OR 'Assembly::Namespace.Class.Method'. Will not run if methodOnStart hasn't run yet at the time of destruction.")]
        private string methodOnDestroy;

        private bool hasInvoked = false;
        float timer = 0f;

        void Update()
        {
            if (!hasInvoked)
            {
                timer += Time.unscaledDeltaTime;
                if (timer >= onStartDelaySeconds)
                {
                    hasInvoked = true;
                    ReflectionHelper.InvokeStaticMethod(methodOnStart);
                }
            }
        }

        void OnDestroy()
        {
            if (!hasInvoked)
                return;

            ReflectionHelper.InvokeStaticMethod(methodOnDestroy);
        }
    }
}
