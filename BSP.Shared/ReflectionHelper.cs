using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public static class ReflectionHelper
    {
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