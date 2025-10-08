using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

namespace tarkin.BSP.Shared
{
    public class SceneObjectDisabler : MonoBehaviour
    {
        public Mode mode;
        public string targetSceneName = "bunker_2";

        public enum Mode
        {
            DisablePermanent,
            DisableTemporary,
            Destroy
        }

        public List<string> pathsToDisable;

        public float delay;

        Dictionary<GameObject, bool> originalStates = new Dictionary<GameObject, bool>();

        void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            CheckAndRunForAlreadyLoadedScene();
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (mode == Mode.DisableTemporary)
            {
                foreach (var kvp in originalStates)
                {
                    kvp.Key.SetActive(kvp.Value);
                }
            }
        }

        private void CheckAndRunForAlreadyLoadedScene()
        {
            Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
            if (targetScene.isLoaded)
            {
                StartCoroutine(DisableObjectsRoutine());
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == targetSceneName)
            {
                StartCoroutine(DisableObjectsRoutine());
            }
        }

        private IEnumerator DisableObjectsRoutine()
        {
            // just in case
            yield return new WaitForEndOfFrame();

            if (delay > 0)
                yield return new WaitForSecondsRealtime(delay);

            Debug.Log($"[{nameof(SceneObjectDisabler)}] Processing {pathsToDisable.Count} paths...");
            int successCount = 0;

            foreach (string path in pathsToDisable)
            {
                GameObject targetObject = FindObjectByPath(path);

                if (targetObject != null)
                {
                    if (mode == Mode.Destroy)
                        Destroy(targetObject);
                    else
                    {
                        if (mode == Mode.DisableTemporary)
                            originalStates.Add(targetObject, targetObject.activeSelf);

                        targetObject.SetActive(false);
                    }

                    successCount++;
                }
                else
                {
                    Debug.LogWarning($"[{nameof(SceneObjectDisabler)}] Could not find object at path: {path}");
                }
            }

            Debug.Log($"[{nameof(SceneObjectDisabler)}] Finished processing. Successfully disabled {successCount}/{pathsToDisable.Count} objects.");
        }

        // a custom function instead of GameObject.Find()
        // for more control and to include inactive root objects in the search
        public static GameObject FindObjectByPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            // Standardize the path by removing a leading slash if it exists, just to be safe
            if (path.StartsWith("/"))
                path = path.Substring(1);

            string[] names = path.Split('/');

            GameObject root = null;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded)
                    continue;

                foreach (var go in scene.GetRootGameObjects())
                {
                    if (go.name == names[0])
                    {
                        root = go;
                        break;
                    }
                }
                if (root != null) break;
            }

            if (root == null)
                return null;

            if (names.Length == 1)
                return root;

            Transform currentTransform = root.transform;
            for (int i = 1; i < names.Length; i++)
            {
                Transform child = currentTransform.Find(names[i]);
                if (child == null)
                {
                    return null;
                }
                currentTransform = child;
            }

            return currentTransform.gameObject;
        }
    }
}