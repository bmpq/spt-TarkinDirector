using UnityEngine;
using System;
using System.Reflection;

namespace tarkin.BSP.Shared
{
    public class StaticMethodInvoker : MonoBehaviour
    {
        [Tooltip("The full name of the class (Namespace.ClassName). No Assembly needed.")]
        [SerializeField] private string typeName;
        [SerializeField] private string methodName;

        [SerializeField] private TriggerType triggerType = TriggerType.OnEnable;

        private enum TriggerType
        {
            OnEnable,
            OnDisable,
            Both,
            None
        }

        private void OnEnable()
        {
            if (triggerType == TriggerType.OnEnable || triggerType == TriggerType.Both)
            {
                InvokeMethod();
            }
        }

        private void OnDisable()
        {
            if (triggerType == TriggerType.OnDisable || triggerType == TriggerType.Both)
            {
                InvokeMethod();
            }
        }

        public void InvokeMethod()
        {
            if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(methodName))
            {
                return;
            }

            Type targetType = FindTypeInAllAssemblies(typeName);

            if (targetType == null)
            {
                Debug.LogError($"[StaticMethodInvoker] Could not find type '{typeName}' in any loaded assembly. Check spelling and Namespace.");
                return;
            }

            MethodInfo methodInfo = targetType.GetMethod(
                methodName,
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy
            );

            if (methodInfo == null)
            {
                Debug.LogError($"[StaticMethodInvoker] Could not find static method '{methodName}' in type '{targetType.FullName}'.");
                return;
            }

            try
            {
                methodInfo.Invoke(null, null);
            }
            catch (Exception e)
            {
                Debug.LogError($"[StaticMethodInvoker] Error invoking '{targetType.FullName}.{methodName}': {e.Message}");
            }
        }

        private Type FindTypeInAllAssemblies(string className)
        {
            Type type = Type.GetType(className);
            if (type != null) return type;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(className);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}
