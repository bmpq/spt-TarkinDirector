using System.Collections.Generic;
using UnityEngine;

namespace tarkin.BSP.Shared
{
    public class HierarchyPathDisabler : MonoBehaviour
    {
        public List<string> targetHierarchyPaths = new List<string>();

        List<GameObject> disabledGameObjects = new List<GameObject>();

        void OnEnable()
        {
            foreach (var path in targetHierarchyPaths)
            {
                GameObject objectToDisable = GameObject.Find(path);
                if (objectToDisable != null && objectToDisable.activeInHierarchy)
                {
                    objectToDisable.SetActive(false);
                    Debug.Log($"Disabled GameObject at path: {path}");

                    disabledGameObjects.Add(objectToDisable);
                }
                else
                {
                    Debug.LogWarning($"Could not find GameObject to disable at path: {path}");
                }
            }
        }

        void OnDisable()
        {
            foreach (var go in disabledGameObjects)
            {
                go.SetActive(true);
            }

            Debug.Log($"Restored {disabledGameObjects.Count} GameObjects");
            disabledGameObjects.Clear();
        }
    }
}
