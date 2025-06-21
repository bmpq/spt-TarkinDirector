using Comfort.Common;
using EFT;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace tarkin.bundlesceneplayer
{
    internal class BundleScenePlayer : MonoBehaviour
    {
        private static Dictionary<string, (AssetBundle, Scene)> loadedAssetBundles = [];

        Coroutine operation;

        void Update()
        {
            if (Input.GetKeyDown(Plugin.KeybindPlayback.Value.MainKey))
            {
                if (operation == null)
                {
                    operation = StartCoroutine(PlaybackBundle(Plugin.BundleName.Value));
                }
                else
                {
                    NotificationManagerClass.DisplayWarningNotification($"Busy!");
                }
            }
        }

        IEnumerator PlaybackBundle(string bundleName)
        {
            if (!Singleton<GameWorld>.Instantiated)
            {
                NotificationManagerClass.DisplayWarningNotification($"No game world!");
                operation = null;
                yield break;
            }

            string gameDirectory = Path.GetDirectoryName(Application.dataPath);
            string relativePath = Path.Combine(Plugin.AddPathToApplicationDataPath, bundleName);
            string fullPath = Path.Combine(gameDirectory, relativePath);

            string key = System.IO.Path.GetFileName(fullPath);

            // unloading the bundle and scene is intended, the bundle file could have been changed during runtime from outside
            if (loadedAssetBundles.ContainsKey(key))
            {
                NotificationManagerClass.DisplayMessageNotification($"Unloading '{bundleName}'");

                AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(loadedAssetBundles[key].Item2);

                while (!asyncOperation.isDone)
                {
                    yield return null;
                }

                loadedAssetBundles[key].Item1.Unload(true);
                loadedAssetBundles.Remove(key);
            }

            if (!File.Exists(fullPath))
            {
                NotificationManagerClass.DisplayWarningNotification($"'{bundleName}' doesn't exist!");
                operation = null;
                yield break;
            }

            AssetBundle assetBundle = AssetBundle.LoadFromFile(fullPath);
            if (assetBundle == null)
            {
                NotificationManagerClass.DisplayWarningNotification($"Error loading '{bundleName}'!");
                operation = null;
                yield break;
            }

            string[] scenePaths = assetBundle.GetAllScenePaths();
            if (scenePaths.Length == 0)
            {
                NotificationManagerClass.DisplayWarningNotification($"'{bundleName}' is not a scene bundle!");
                operation = null;
                yield break;
            }

            string sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!asyncOp.isDone)
                {
                    yield return null;
                }
                Scene loadedScene = SceneManager.GetSceneByName(sceneName);

                ReplaceShadersToNative(loadedScene);

                loadedAssetBundles.Add(key, (assetBundle, loadedScene));
            }

            NotificationManagerClass.DisplayMessageNotification($"'{bundleName}': Playback started");
            operation = null;
        }

        private static void ReplaceShadersToNative(Scene loadedScene)
        {
            foreach (GameObject rootGameObject in loadedScene.GetRootGameObjects())
            {
                foreach (var rend in rootGameObject.GetComponentsInChildren<Renderer>())
                {
                    foreach (var mat in rend.materials)
                    {
                        if (mat == null || mat.shader == null)
                            continue;

                        Shader nativeShader = Shader.Find(mat.shader.name);
                        if (nativeShader != null)
                        {
                            mat.shader = nativeShader;
                            Debug.Log($"Replaced shader '{mat.shader.name}'");
                        }
                        else
                            Debug.LogError($"shader '{mat.shader.name}' not found!");
                    }
                }
            }
        }
    }
}
