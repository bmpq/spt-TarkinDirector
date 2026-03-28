using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace tarkin.Director
{
    public static class ReflectionHelper
    {
        /// <param name="entry">Namespace.Class.Method OR Assembly::Namespace.Class.Method</param>
        public static void InvokeStaticMethod(string entry)
        {
            if (string.IsNullOrEmpty(entry))
                return;

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

        public static bool InvokeStaticMethod(string targetClass, string targetMethod, string targetAssembly = "")
        {
            if (string.IsNullOrEmpty(targetClass) || string.IsNullOrEmpty(targetMethod))
                return false;

            Type type = null;

            if (!string.IsNullOrEmpty(targetAssembly))
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == targetAssembly);

                if (assembly != null)
                {
                    type = assembly.GetType(targetClass);
                }
                else
                {
                    Debug.LogWarning($"ReflectionHelper: Assembly '{targetAssembly}' not found.");
                    return false;
                }
            }
            else
            {
                type = Type.GetType(targetClass);
            }

            if (type == null)
            {
                Debug.LogWarning($"ReflectionHelper: Type '{targetClass}' not found.");
                return false;
            }

            var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            try
            {
                // specifically find methods with NO parameters
                var methodInfo = type.GetMethod(targetMethod, flags, null, Type.EmptyTypes, null);

                if (methodInfo != null)
                {
                    methodInfo.Invoke(null, null);
                    return true;
                }
                else
                {
                    Debug.LogWarning($"ReflectionHelper: Method '{targetMethod}' (parameterless) not found in '{targetClass}'.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"ReflectionHelper: Error invoking {targetClass}.{targetMethod} - {ex.Message}");
            }

            return false;
        }
    }
}