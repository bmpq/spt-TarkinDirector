using System;
using System.Collections.Generic;
using UnityEngine;

namespace tarkin.Director.Interactable
{
    internal class InteractableInvokeReflectionStaticMethod : CustomInteractable
    {
        [SerializeField]
        [Tooltip("Format: 'Namespace.Class.Method' OR 'Assembly::Namespace.Class.Method'")]
        private string[] entries;

        public override IEnumerable<CustomInteractableAction> GetCustomActions()
        {
            List<CustomInteractableAction> actions = new List<CustomInteractableAction>();

            foreach (var entry in entries)
            {
                if (!string.IsNullOrEmpty(entry))
                {
                    actions.Add(CustomInteractableAction.Generic(entry));
                }
            }

            return actions;
        }

        public override void ExecuteAction(string actionName, Action finishCallback)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i] == actionName)
                {
                    if (!string.IsNullOrEmpty(entries[i]))
                    {
                        ReflectionHelper.InvokeStaticMethod(entries[i]);
                    }
                    break;
                }
            }

            finishCallback?.Invoke();
        }

    }
}