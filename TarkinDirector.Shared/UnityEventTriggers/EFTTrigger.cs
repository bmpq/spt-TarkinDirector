using System;
using UnityEngine;
using UnityEngine.Events;

namespace tarkin.Director
{
    public class EFTTrigger : MonoBehaviour
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

        [SerializeField]
        private UnityEvent unityEvent;

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
                unityEvent.Invoke();
        }
    }
}
