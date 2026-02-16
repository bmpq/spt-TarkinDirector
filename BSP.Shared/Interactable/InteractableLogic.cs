using System;
using System.Collections.Generic;
using UnityEngine;

namespace tarkin.BSP.Shared.Interactable
{
    public abstract class InteractableLogic : MonoBehaviour
    {
        public static event Action<InteractableLogic> OnAwake;
        private void Awake() { OnAwake?.Invoke(this); }
        public abstract IEnumerable<InteractableAction> GetActions();
        public abstract void ExecuteAction(string actionName, Action finishCallback);
        public virtual event Action OnStateChanged;
    }
}
