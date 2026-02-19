using System;
using System.Collections.Generic;
using UnityEngine;

namespace tarkin.BSP.Shared.Interactable
{
    internal class InteractableInvokeReflectionStaticMethod : InteractableLogic
    {
        [SerializeField]
        [Tooltip("Format: 'Namespace.Class.Method' OR 'Assembly::Namespace.Class.Method'")]
        private string[] entries;

        public override IEnumerable<InteractableAction> GetActions()
        {
            List<InteractableAction> actions = new List<InteractableAction>();

            foreach (var entry in entries)
            {
                if (!string.IsNullOrEmpty(entry))
                {
                    actions.Add(InteractableAction.Generic(entry));
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
                        ParseAndInvoke(entries[i]);
                    }
                    break;
                }
            }

            finishCallback?.Invoke();
        }

        private void ParseAndInvoke(string entry)
        {
            string targetAssembly = "";
            string classAndMethod = entry;

            // check for explicit Assembly definition
            int assemblySeparator = entry.IndexOf("::", StringComparison.Ordinal);
            if (assemblySeparator != -1)
            {
                targetAssembly = entry.Substring(0, assemblySeparator);
                classAndMethod = entry.Substring(assemblySeparator + 2);
            }

            // separate Class from Method (by last dot)
            int lastDotIndex = classAndMethod.LastIndexOf('.');

            if (lastDotIndex != -1 && lastDotIndex < classAndMethod.Length - 1)
            {
                string targetClass = classAndMethod.Substring(0, lastDotIndex);
                string targetMethod = classAndMethod.Substring(lastDotIndex + 1);

                bool success = ReflectionHelper.InvokeStaticMethod(targetClass, targetMethod, targetAssembly);

                if (!success)
                {
                    Debug.LogWarning($"[InteractableInvoke] Failed to invoke: {entry}. Check class/method names and assembly.");
                }
            }
            else
            {
                Debug.LogError($"[InteractableInvoke] Invalid format for '{entry}'. Expected 'Namespace.Class.Method'");
            }
        }
    }
}