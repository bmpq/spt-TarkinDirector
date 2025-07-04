using Comfort.Common;
using EFT;
using EFT.CameraControl;
using EFT.Interactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using tarkin.BSP.BepInEx.Patches;
using tarkin.BSP.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace tarkin.BSP.BepInEx
{
    internal class BundleScenePlayer : MonoBehaviour
    {
        private class LoadedBundleInfo
        {
            public AssetBundle Bundle { get; }
            public Scene Scene { get; }

            public LoadedBundleInfo(AssetBundle bundle, Scene scene)
            {
                Bundle = bundle;
                Scene = scene;
            }
        }

        private Dictionary<string, LoadedBundleInfo> loadedAssetBundles = new Dictionary<string, LoadedBundleInfo>();

        Coroutine operation;

        Camera animatedCamera;
        float cameraOverrideFactor;
        bool cameraOverride;

        void Update()
        {
            if (Input.GetKeyDown(Plugin.KeybindUnloadAll.Value.MainKey))
            {
                if (operation == null)
                {
                    operation = StartCoroutine(UnloadAllBundlesRoutine());
                }
                else
                {
                    NotificationManagerClass.DisplayWarningNotification($"Busy!");
                }
            }

            if (Input.GetKeyDown(Plugin.KeybindPlayback.Value.MainKey))
            {
                if (operation == null)
                {
                    operation = StartCoroutine(ReloadBundle(Plugin.BundleFullPath));
                }
                else
                {
                    NotificationManagerClass.DisplayWarningNotification($"Busy!");
                }
            }

            if (Input.GetKeyDown(Plugin.KeybindToggleCameraOverride.Value.MainKey))
            {
                cameraOverrideFactor = 0;

                cameraOverride = !cameraOverride;
                if (animatedCamera == null)
                    cameraOverride = false;

                TogglePlayerCameraController(!cameraOverride);
            }

            if (cameraOverride && animatedCamera != null && CameraClass.Instance.Camera != null)
            {
                if (cameraOverrideFactor < 1f)
                {
                    cameraOverrideFactor += Time.deltaTime * Plugin.CameraOverrideHandoverSpeed.Value;
                    if (cameraOverrideFactor > 1f)
                        cameraOverrideFactor = 1f;
                }

                TransformGameCameraToBundleCamera(cameraOverrideFactor);
            }
        }

        void TransformGameCameraToBundleCamera(float t)
        {
            CameraClass.Instance.Camera.transform.position = Vector3.Lerp(CameraClass.Instance.Camera.transform.position, animatedCamera.transform.position, t);
            CameraClass.Instance.Camera.transform.rotation = Quaternion.Lerp(CameraClass.Instance.Camera.transform.rotation, animatedCamera.transform.rotation, t);
            CameraClass.Instance.Camera.fieldOfView = Mathf.Lerp(CameraClass.Instance.Camera.fieldOfView, animatedCamera.fieldOfView, t);
        }

        IEnumerator UnloadAllBundlesRoutine()
        {
            try
            {
                List<string> paths = loadedAssetBundles.Keys.ToList();
                foreach (var path in paths)
                {
                    yield return StartCoroutine(UnloadBundleRoutine(path));
                }
            }
            finally
            {
                operation = null;
            }
        }

        IEnumerator ReloadBundle(string fullPath)
        {
            try
            {
                if (loadedAssetBundles.ContainsKey(fullPath))
                {
                    yield return StartCoroutine(UnloadBundleRoutine(fullPath));
                    NotificationManagerClass.DisplayMessageNotification($"'{Path.GetFileName(fullPath)}' unloaded.");
                }
                
                yield return StartCoroutine(LoadBundleRoutine(fullPath));
            }
            finally
            {
                operation = null;
            }
        }
        
        IEnumerator UnloadBundleRoutine(string fullPath)
        {
            if (!loadedAssetBundles.TryGetValue(fullPath, out var info))
            {
                yield break;
            }

            NotificationManagerClass.DisplayMessageNotification($"Unloading '{Path.GetFileName(fullPath)}'...");
            
            if (animatedCamera != null)
            {
                animatedCamera = null;
            }

            if (info.Scene.isLoaded)
            {
                AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(info.Scene);
                while (!asyncOperation.isDone)
                {
                    yield return null;
                }
            }

            info.Bundle.Unload(true);
            loadedAssetBundles.Remove(fullPath);
        }

        IEnumerator LoadBundleRoutine(string fullPath)
        {
            if (!Singleton<GameWorld>.Instantiated)
            {
                NotificationManagerClass.DisplayWarningNotification("Cannot load bundle: Not in raid!");
                yield break;
            }

            if (!File.Exists(fullPath))
            {
                NotificationManagerClass.DisplayWarningNotification($"Bundle not found: '{Path.GetFileName(fullPath)}'");
                yield break;
            }

            NotificationManagerClass.DisplayMessageNotification($"Loading '{Path.GetFileName(fullPath)}'...");

            AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(fullPath);
            yield return bundleRequest;

            AssetBundle assetBundle = bundleRequest.assetBundle;
            if (assetBundle == null)
            {
                NotificationManagerClass.DisplayWarningNotification($"Error loading asset bundle!");
                yield break;
            }

            string[] scenePaths = assetBundle.GetAllScenePaths();
            if (scenePaths.Length == 0)
            {
                NotificationManagerClass.DisplayWarningNotification($"'{Path.GetFileName(fullPath)}' is not a scene bundle!");
                assetBundle.Unload(false);
                yield break;
            }

            string sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);
            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!asyncOp.isDone)
            {
                yield return null;
            }

            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            if (!loadedScene.isLoaded)
            {
                NotificationManagerClass.DisplayWarningNotification($"Failed to load scene '{sceneName}' from bundle.");
                assetBundle.Unload(true);
                yield break;
            }

            var bundleInfo = new LoadedBundleInfo(assetBundle, loadedScene);
            loadedAssetBundles.Add(fullPath, bundleInfo);

            ReplaceShadersToNative(loadedScene);
            ParseAndSubscribeTriggers(loadedScene);

            animatedCamera = FindSceneCamera(loadedScene);
            if (animatedCamera != null)
            {
                animatedCamera.enabled = false;
            }

            Physics.simulationMode = SimulationMode.FixedUpdate;

            NotificationManagerClass.DisplayMessageNotification($"'{Path.GetFileName(fullPath)}': Scene loaded successfully.");
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

        private void ParseAndSubscribeTriggers(Scene scene)
        {
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                foreach (var trigger in rootGameObject.GetComponentsInChildren<EFTTrigger>(true))
                {
                    switch (trigger.trigger)
                    {
                        case EFTTrigger.Trigger.DoorBreach:
                            {
                                Patch_Door_KickOpen.OnPostfix += trigger.Execute;
                                trigger.OnDestroyAction = () => Patch_Door_KickOpen.OnPostfix -= trigger.Execute;
                                break;
                            }
                        case EFTTrigger.Trigger.DoorOpen:
                            {
                                Action<EDoorState> action = (state) => { if (state == EDoorState.Open) trigger.Execute(); };
                                Patch_WorldInteractiveObject_DoorStateChanged.OnPostfix += action;
                                trigger.OnDestroyAction = () => Patch_WorldInteractiveObject_DoorStateChanged.OnPostfix -= action;
                                break;
                            }
                        case EFTTrigger.Trigger.DoorShut:
                            {
                                Action<EDoorState> action = (state) => { if (state == EDoorState.Shut) trigger.Execute(); };
                                Patch_WorldInteractiveObject_DoorStateChanged.OnPostfix += action;
                                trigger.OnDestroyAction = () => Patch_WorldInteractiveObject_DoorStateChanged.OnPostfix -= action;
                                break;
                            }
                    }
                }
            }
        }

        Camera FindSceneCamera(Scene scene)
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
