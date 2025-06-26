using UnityEngine;

namespace tarkin.BSP.Shared
{
    internal class InputTrigger : AnimatorAction
    {
        public KeyCode keycode;

        void Update()
        {
            if (Input.GetKeyDown(keycode))
                Invoke();
        }
    }
}
