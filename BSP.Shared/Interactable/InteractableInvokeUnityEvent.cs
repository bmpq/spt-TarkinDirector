using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace tarkin.BSP.Shared.Interactable
{
    internal class InteractableInvokeUnityEvent : InteractableLogic
    {
        [SerializeField]
        private UnityEvent[] entries;

        [SerializeField]
        private string[] entryEFTActions;

        public override IEnumerable<InteractableAction> GetActions()
        {
            List<InteractableAction> actions = new List<InteractableAction>();

            foreach (var entryName in entryEFTActions)
            {
                if (!string.IsNullOrEmpty(entryName))
                {
                    actions.Add(InteractableAction.Generic(entryName));
                }
            }

            return actions;
        }

        public override void ExecuteAction(string actionName, Action finishCallback)
        {
            for (int i = 0; i < entryEFTActions.Length; i++)
            {
                if (entryEFTActions[i] == actionName)
                {
                    if (i < entries.Length && entries[i] != null)
                    {
                        entries[i].Invoke();
                    }
                    break;
                }
            }

            finishCallback?.Invoke();
        }
    }
}
