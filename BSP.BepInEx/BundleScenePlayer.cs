using Comfort.Common;
using EFT;
using EFT.CameraControl;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Systems.Effects;
using tarkin.BSP.Bep.Patches;
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

        private readonly Dictionary<string, LoadedBundleInfo> loadedAssetBundles = new Dictionary<string, LoadedBundleInfo>();
        private readonly ConcurrentQueue<string> changedFilesQueue = new ConcurrentQueue<string>();
        private FileSystemWatcher fileWatcher;

        Coroutine operation;

        List<Camera> cameraProxies = new List<Camera>();
        float cameraOverrideFactor;
        bool cameraOverride;
        RenderTexture dummyRenderTexture;

        void Start()
        {
            dummyRenderTexture = new RenderTexture(1, 1, 16);
            dummyRenderTexture.Create();

            SetupFileWatcher();
        }

        void OnDestroy()
        {
            fileWatcher?.Dispose();
        }

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
                    if (!Plugin.Silent.Value)
                    NotificationManagerClass.DisplayWarningNotification($"Busy!");
                }
            }

            ProcessChangedFilesQueue();

            if (Input.GetKeyDown(Plugin.KeybindToggleCameraOverride.Value.MainKey))
            {
                cameraOverrideFactor = 0;

                cameraOverride = !cameraOverride;
                if (cameraProxies.Count == 0)
                    cameraOverride = false;

                ToggleEnvUICam(!cameraOverride);
                TogglePlayerCameraController(!cameraOverride);
            }

            if (cameraOverride && cameraProxies.Count > 0 && CameraClass.Instance.Camera != null)
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

        private void ToggleEnvUICam(bool value)
        {
            Patch_EnvironmentUIRoot_SetCameraActive.CameraContainer?.gameObject.SetActive(value);
        }

        private void SetupFileWatcher()
        {
            string bundleDirectoryPath = Path.GetDirectoryName(Plugin.BundleFullPath);

            if (!Directory.Exists(bundleDirectoryPath))
            {
                Plugin.Log.LogWarning($"Cannot monitor bundle directory as it does not exist: {bundleDirectoryPath}");
                return;
            }

            fileWatcher = new FileSystemWatcher(bundleDirectoryPath)
            {
                NotifyFilter = NotifyFilters.LastWrite
            };

            fileWatcher.Changed += OnFileChanged;
            fileWatcher.Created += OnFileChanged;

            fileWatcher.EnableRaisingEvents = true;
            Plugin.Log.LogInfo($"File watcher active for: {bundleDirectoryPath}");
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (!Plugin.MonitorForChanges.Value)
                return;

            if (loadedAssetBundles.ContainsKey(e.FullPath))
            {
                changedFilesQueue.Enqueue(e.FullPath);
            }
        }

        private void ProcessChangedFilesQueue()
        {
            if (operation != null) return;

            if (changedFilesQueue.TryDequeue(out string fullPath))
            {
                if (loadedAssetBundles.ContainsKey(fullPath))
                {
                    if (!Plugin.Silent.Value)
                    {
                        NotificationManagerClass.DisplayMessageNotification($"File change detected. Reloading '{Path.GetFileName(fullPath)}'...");
                    }
                    operation = StartCoroutine(ReloadBundle(fullPath));
                }
            }
        }

        void TransformGameCameraToBundleCamera(float t)
        {
            Camera activeProxyCamera = cameraProxies[0];

            foreach (var proxyCam in cameraProxies)
            {
                if (proxyCam.isActiveAndEnabled)
                {
                    activeProxyCamera = proxyCam;
                }
            }

            if (activeProxyCamera == null)
                return;

            if (CameraClass.Instance.Camera == null)
                return;

            CameraClass.Instance.Camera.transform.position = Vector3.Lerp(CameraClass.Instance.Camera.transform.position, activeProxyCamera.transform.position, t);
            CameraClass.Instance.Camera.transform.rotation = Quaternion.Lerp(CameraClass.Instance.Camera.transform.rotation, activeProxyCamera.transform.rotation, t);
            CameraClass.Instance.Camera.fieldOfView = Mathf.Lerp(CameraClass.Instance.Camera.fieldOfView, activeProxyCamera.fieldOfView, t);
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
                    if (!Plugin.Silent.Value)
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

            if (!Plugin.Silent.Value)
                NotificationManagerClass.DisplayMessageNotification($"Unloading '{Path.GetFileName(fullPath)}'...");

            cameraProxies.Clear();

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

            Resources.UnloadUnusedAssets();
        }

        IEnumerator LoadBundleRoutine(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                NotificationManagerClass.DisplayWarningNotification($"Bundle not found: '{Path.GetFileName(fullPath)}'");
                yield break;
            }

            if (!Plugin.Silent.Value)
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

            cameraProxies = FindSceneCameras(loadedScene);

            var bundleInfo = new LoadedBundleInfo(assetBundle, loadedScene);
            loadedAssetBundles.Add(fullPath, bundleInfo);

            ReplaceShadersToNative(loadedScene);

            if (Plugin.CleanDecals.Value)
            {
                Singleton<Effects>.Instance?.ClearDecal();
            }

            if (!Plugin.Silent.Value)
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

        List<Camera> FindSceneCameras(Scene scene)
        {
            List<Camera> cameraProxies = new List<Camera>();
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                cameraProxies.AddRange(rootGameObject.GetComponentsInChildren<Camera>(true));
            }
            return cameraProxies;

            foreach (Camera cam in cameraProxies)
            {
                cam.cullingMask = 0;
                cam.targetTexture = dummyRenderTexture;
            }

            return cameraProxies;
        }

        void ReplaceShadersToNative(Scene scene)
        {
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                foreach (Renderer rend in rootGameObject.GetComponentsInChildren<Renderer>(true))
                {
                    foreach (Material mat in rend.sharedMaterials)
                    {
                        if (mat != null && mat.shader != null)
                        {
                            Shader nativeShader = Shader.Find(mat.shader.name);
                            if (nativeShader != null)
                                mat.shader = nativeShader;
                        }
                    }
                }
            }
        }
    }
}
