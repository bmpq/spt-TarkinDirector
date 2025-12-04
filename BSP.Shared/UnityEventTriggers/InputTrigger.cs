using UnityEngine;
using UnityEngine.Events;

namespace tarkin.BSP.Shared
{
    internal class InputTrigger : MonoBehaviour
    {
        [Space(10)]
        public KeyCode keycode;

        [SerializeField]
        private UnityEvent unityEvent;

        void Update()
        {
            if (Input.GetKeyDown(keycode))
                unityEvent.Invoke();
        }
    }
}
