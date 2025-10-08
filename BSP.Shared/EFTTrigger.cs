using System;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class EFTTrigger : AnimatorAction
    {
        public static void SendEFTEvent(Trigger trigger)
        {
            onEFTEvent?.Invoke(trigger);
        }

        private static Action<Trigger> onEFTEvent;

        public enum Trigger
        { 
            DoorBreach,
            DoorOpen,
            DoorShut
        }

        [SerializeField]
        private Trigger trigger;

        void OnEnable()
        {
            onEFTEvent += ReceiveEvent;
        }

        void OnDisable()
        {
            onEFTEvent -= ReceiveEvent;
        }

        void ReceiveEvent(Trigger trigger)
        {
            if (this.trigger == trigger)
                Invoke();
        }
    }
}
