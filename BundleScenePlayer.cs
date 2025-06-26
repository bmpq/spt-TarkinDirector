using Comfort.Common;
using EFT;
using EFT.CameraControl;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using tarkin.bundlesceneplayer.Patches;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace tarkin.bundlesceneplayer
{
    internal class BundleScenePlayer : MonoBehaviour
    {
        private static Dictionary<string, (AssetBundle, Scene)> loadedAssetBundles = [];

        Coroutine operation;

        Camera animatedCamera;

        void Start()
        {
            Patch_Door_KickOpen.OnPostfix += () => 
            { 
                if (Plugin.Trigger.Value == PlaybackTrigger.DoorBreach)
                    TriggerPlayback();
            };
        }

        private void TriggerPlayback()
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

        void Update()
        {
            if (Input.GetKeyDown(Plugin.KeybindPlayback.Value.MainKey))
            {
                TriggerPlayback();
            }

            if (Input.GetKeyDown(Plugin.KeybindReleaseAnimatedCamera.Value.MainKey))
            {
                animatedCamera = null;
                TogglePlayerCameraController(true);
            }

            if (animatedCamera != null && CameraClass.Instance.Camera != null)
            {
                CameraClass.Instance.Camera.transform.position = animatedCamera.transform.position;
                CameraClass.Instance.Camera.transform.rotation = animatedCamera.transform.rotation;
                CameraClass.Instance.Camera.fieldOfView = animatedCamera.fieldOfView;
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

                animatedCamera = CheckSceneCamera(loadedScene);
                if (animatedCamera != null)
                {
                    animatedCamera.enabled = false;
                    TogglePlayerCameraController(false);
                }

                loadedAssetBundles.Add(key, (assetBundle, loadedScene));
            }

            NotificationManagerClass.DisplayMessageNotification($"'{bundleName}': Playback started");
            operation = null;
        }

        void TogglePlayerCameraController(bool on)
        {
            Player player = Singleton<GameWorld>.Instance?.MainPlayer;
            if (on && player != null && player.TryGetComponent<HideoutPlayerOwner>(out var hideoutPlayerOwner))
            {
                if (!hideoutPlayerOwner.FirstPersonMode)
                    return;
            }

            PlayerCameraController playerCameraController = player?.gameObject.GetComponent<PlayerCameraController>();
            if (playerCameraController != null)
            {
                playerCameraController.enabled = on;
            }
        }

        Camera CheckSceneCamera(Scene scene)
        {
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                Camera sceneCamera = rootGameObject.GetComponentInChildren<Camera>();
                if (sceneCamera != null)
                    return sceneCamera;
            }

            return null;
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
