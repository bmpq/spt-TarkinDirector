using System;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class EFTTrigger : AnimatorAction
    {
        public enum Trigger
        { 
            DoorBreach,
            DoorOpen,
            DoorShut
        }

        public Trigger trigger;
        public Action OnDestroyAction;

        public void Execute()
        {
            Invoke();
        }

        private void OnDestroy()
        {
            OnDestroyAction?.Invoke();
        }
    }
}
