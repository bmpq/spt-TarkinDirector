using Comfort.Common;
using EFT;
using EFT.CameraControl;
using EFT.Interactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using tarkin.BSP.Bep.Patches;
using tarkin.BSP.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace tarkin.BSP.Bep
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
                    PrewarmAssemblies();

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

        private void PrewarmAssemblies()
        {
            var mainAss = typeof(MyMovingPlatform);

            string[] assembliesToLoad = Plugin.PrewarmAssemblies.Value.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
            foreach (var assName in assembliesToLoad)
            {
                try
                {
                    string fullPath = Path.Combine(BepInEx.Paths.PluginPath, Plugin.AddAssembliesPathToPluginPath, assName + ".dll");
                    if (File.Exists(fullPath))
                    {
                        Assembly asm = Assembly.LoadFrom(fullPath);
                    }
                    else
                    {
                        Plugin.Log.LogError($"File does not exist: {fullPath}");
                        continue;
                    }
                }
                catch
                {
                    Plugin.Log.LogError($"BSP: Failed to load assembly {assName}.dll!");
                    continue;
                }
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
                assetBundle?.Unload(false);
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
                assetBundle?.Unload(true);
                yield break;
            }

            try
            {
                ParseSceneSettings(loadedScene);
                ParseAndSubscribeTriggers(loadedScene);
                ParseMovingPlatforms(loadedScene);

                animatedCamera = FindSceneCamera(loadedScene);
                if (animatedCamera != null)
                {
                    animatedCamera.enabled = false;
                }
            }
            catch (Exception e)
            {
                NotificationManagerClass.DisplayWarningNotification($"Failed to init scene '{sceneName}'");
                NotificationManagerClass.DisplayWarningNotification($"{e.Message}");
                assetBundle?.Unload(true);
                yield break;
            }

            var bundleInfo = new LoadedBundleInfo(assetBundle, loadedScene);
            loadedAssetBundles.Add(fullPath, bundleInfo);

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

        private void ParseSceneSettings(Scene scene)
        {
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                if (rootGameObject.TryGetComponent<SceneSettings>(out var settings))
                {
                    Physics.simulationMode = settings.physicsMode;
                    return;
                }
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


        // doing this because the shared assembly doesn't depend on game assembly (doesn't know about Player)
        class MovingPlatformMediator
        {
            private readonly List<Player> passengers = [];

            public MovingPlatformMediator(MyMovingPlatform platform)
            {
                platform.ActionOnTriggerEnter += OnTriggerEnter;
                platform.ActionOnTriggerExit += OnTriggerExit;
                platform.ActionLateUpdatePositionDelta += LateUpdatePositionDelta;
            }

            void OnTriggerEnter(Collider col)
            {
                if (col.gameObject.layer == LayerMaskClass.PlayerLayer && col.gameObject.TryGetComponent<Player>(out Player player))
                {
                    if (!passengers.Contains(player))
                        passengers.Add(player);
                }
            }

            void OnTriggerExit(Collider col)
            {
                if (col.gameObject.layer == LayerMaskClass.PlayerLayer && col.gameObject.TryGetComponent<Player>(out Player player))
                {
                    if (passengers.Contains(player))
                        passengers.Remove(player);
                }
            }

            void LateUpdatePositionDelta(Vector3 delta)
            {
                foreach (var passenger in passengers)
                {
                    if (passenger != null && passenger.MovementContext != null)
                        passenger.MovementContext.PlatformMotion = delta;
                }
            }
        }

        HashSet<MovingPlatformMediator> movingPlatforms = [];

        private void ParseMovingPlatforms(Scene scene)
        {
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                foreach (var item in rootGameObject.GetComponentsInChildren<MyMovingPlatform>(true))
                {
                    MovingPlatformMediator movingPlatformMediator = new MovingPlatformMediator(item);
                    movingPlatforms.Add(movingPlatformMediator); 
                    item.ActionOnDestroy += () => movingPlatforms.Remove(movingPlatformMediator);
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
    }
}
