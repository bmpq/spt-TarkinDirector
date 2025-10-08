using UnityEngine;

namespace tarkin.BSP.Shared
{
    internal class InputTrigger : AnimatorAction
    {
        [Space(10)]
        public KeyCode keycode;

        void Update()
        {
            if (Input.GetKeyDown(keycode))
                Invoke();
        }
    }
}
