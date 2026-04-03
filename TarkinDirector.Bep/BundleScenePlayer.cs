using Comfort.Common;
using EFT;
using EFT.CameraControl;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Systems.Effects;
using tarkin.Director.Bep;
using tarkin.Director.Bep.Patches;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace tarkin.Director.EFTRuntime
{
    [DefaultExecutionOrder(1000)]
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

        Coroutine operation;

        List<Camera> cameraProxies = new List<Camera>();
        float cameraOverrideFactor;
        bool cameraOverride;
        RenderTexture dummyRenderTexture;

        void Start()
        {
            dummyRenderTexture = new RenderTexture(1, 1, 16);
            dummyRenderTexture.Create();
        }

        void LateUpdate()
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
                    var pathsToLoad = Plugin.GetConfiguredBundlePaths();

                    if (pathsToLoad.Count > 0)
                    {
                        operation = StartCoroutine(ReloadBundlesSequence(pathsToLoad));
                    }
                    else
                    {
                        NotificationManagerClass.DisplayWarningNotification("No bundles configured in settings!");
                    }
                }
                else
                {
                    if (!Plugin.Silent.Value)
                        NotificationManagerClass.DisplayWarningNotification($"Busy!");
                }
            }

            if (Input.GetKeyDown(Plugin.KeybindToggleCameraOverride.Value.MainKey))
            {
                cameraOverrideFactor = 0;

                cameraOverride = !cameraOverride;
                if (cameraProxies.Count == 0)
                    cameraOverride = false;

                TogglePlayerCameraController(!cameraOverride);
            }

            if (cameraOverride && cameraProxies.Count > 0 && cameraOverrideFactor < 1f)
            {
                if (cameraOverrideFactor < 1f)
                {
                    cameraOverrideFactor += Time.deltaTime * Plugin.CameraOverrideHandoverSpeed.Value;
                    if (cameraOverrideFactor > 1f)
                        cameraOverrideFactor = 1f;
                }
            }

            TransformGameCameraToBundleCamera(cameraOverrideFactor);
        }

        void TransformGameCameraToBundleCamera(float t)
        {
            if (cameraProxies == null || cameraProxies.Count == 0)
            {
                return;
            }

            if (t == 0)
                return;

            Camera activeProxyCamera = cameraProxies[0];
            foreach (var proxyCam in cameraProxies)
            {
                if (proxyCam.isActiveAndEnabled)
                {
                    activeProxyCamera = proxyCam;
                }
            }

            Patch_EnvironmentUIRoot_SetCameraActive.CurrentEnvironmentUIRoot?.SetCameraActive(false);
            Patch_EnvironmentUIRoot_SetCameraActive.CurrentEnvironmentUIRoot?.Shading?.gameObject.SetActive(false);

            if (activeProxyCamera == null)
            {
                Plugin.Log.LogError("No proxy cameras on the loaded scene!");
                return;
            }

            if (CameraClass.Instance.Camera == null)
            {
                CameraClass.Instance.Camera = new GameObject("can").AddComponent<Camera>(); 
                
                CameraClass.Instance.Camera.gameObject.tag = "MainCamera";
            }

            CameraClass.Instance.Camera.gameObject.SetActive(true);
            CameraClass.Instance.Camera.transform.position = Vector3.Lerp(CameraClass.Instance.Camera.transform.position, activeProxyCamera.transform.position, t);
            CameraClass.Instance.Camera.transform.rotation = Quaternion.Lerp(CameraClass.Instance.Camera.transform.rotation, activeProxyCamera.transform.rotation, t);
            CameraClass.Instance.Camera.fieldOfView = Mathf.Lerp(CameraClass.Instance.Camera.fieldOfView, activeProxyCamera.fieldOfView, t);
            CameraClass.Instance.Camera.nearClipPlane = activeProxyCamera.nearClipPlane;
            CameraClass.Instance.Camera.farClipPlane = activeProxyCamera.farClipPlane;
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

        IEnumerator ReloadBundlesSequence(List<string> targetPaths)
        {
            try
            {
                var currentLoadedPaths = loadedAssetBundles.Keys.ToList();
                foreach (var loadedPath in currentLoadedPaths)
                {
                    if (!targetPaths.Contains(loadedPath) || true)
                    {
                        yield return StartCoroutine(UnloadBundleRoutine(loadedPath));
                    }
                }

                yield return new WaitForSecondsRealtime(0.5f);

                foreach (var path in targetPaths)
                {
                    if (loadedAssetBundles.ContainsKey(path)) continue;

                    yield return StartCoroutine(LoadBundleRoutine(path));
                }
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
            Plugin.Log.LogInfo($"found {cameraProxies.Count} camera proxies");

            var bundleInfo = new LoadedBundleInfo(assetBundle, loadedScene);
            loadedAssetBundles.Add(fullPath, bundleInfo);

            ReplaceShadersToNative(loadedScene);

            if (Plugin.SetActiveScene.Value)
                SceneManager.SetActiveScene(loadedScene);

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

            var cinemachine = CameraClass.Instance.Camera?.GetComponent<Cinemachine.CinemachineBrain>();
            if (cinemachine != null)
                cinemachine.enabled = on;

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

            foreach (Camera cam in cameraProxies)
            {
                cam.cullingMask = 0;
                cam.targetTexture = dummyRenderTexture;
            }

            return cameraProxies;
        }

        void ReplaceShadersToNative(Scene scene)
        {
            int replacedShaderCount = 0;

            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                foreach (Renderer rend in rootGameObject.GetComponentsInChildren<Renderer>(true))
                {
                    foreach (Material mat in rend.sharedMaterials)
                    {
                        if (mat != null && mat.shader != null)
                        {
                            Shader nativeShader = Shader.Find(mat.shader.name);
                            if (nativeShader != null && mat.shader != nativeShader)
                            {
                                mat.shader = nativeShader;
                                replacedShaderCount++;
                            }
                        }
                    }
                }
            }

            Plugin.Log.LogInfo($"replaced {replacedShaderCount} shaders");
        }

        void OnDestroy()
        {
            List<string> loadedPaths = loadedAssetBundles.Keys.ToList();

            foreach (string fullPath in loadedPaths)
            {
                LoadedBundleInfo info = loadedAssetBundles[fullPath];
                if (info != null)
                {
                    if (info.Scene.isLoaded)
                        SceneManager.UnloadScene(info.Scene);
                    info.Bundle.Unload(false);
                }

                loadedAssetBundles.Remove(fullPath);

                if (!Plugin.Silent.Value)
                    NotificationManagerClass.DisplayMessageNotification($"Unloading '{Path.GetFileName(fullPath)}'...");
            }

            cameraProxies.Clear();
        }
    }
}
