using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.IO;
using tarkin.BSP.BepInEx.Patches;
using UnityEngine;

namespace tarkin.BSP.BepInEx
{
    [BepInPlugin("com.tarkin.bundlesceneplayer", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        public static string AddPathToApplicationDataPath = Path.Combine("BepInEx", "plugins", "tarkin", "bundles");

        internal static new ManualLogSource Log;

        internal static ConfigEntry<string> BundleName;
        internal static ConfigEntry<float> CameraOverrideHandoverSpeed;
        internal static ConfigEntry<KeyboardShortcut> KeybindPlayback;
        internal static ConfigEntry<KeyboardShortcut> KeybindToggleCameraOverride;

        public static string BundleFullPath
        {
            get
            {
                string gameDirectory = Path.GetDirectoryName(Application.dataPath);
                string relativePath = Path.Combine(Plugin.AddPathToApplicationDataPath, Plugin.BundleName.Value);
                string fullPath = Path.Combine(gameDirectory, relativePath);
                return fullPath;
            }
        }

        private void Awake()
        {
            Log = base.Logger;

            InitConfiguration();

            DontDestroyOnLoad(new GameObject("Bundle Scene Player").AddComponent<BundleScenePlayer>().gameObject);

            new Patch_Door_KickOpen().Enable();
            new Patch_WorldInteractiveObject_DoorStateChanged().Enable();
        }

        private void InitConfiguration()
        {
            BundleName = Config.Bind("General", "Bundle name", "scene_buckshot", "");

            CameraOverrideHandoverSpeed = Config.Bind("General", "CameraOverrideHandoverSpeed", 2f, "");

            KeybindPlayback = Config.Bind("Keybinds", "Keybind Playback", new KeyboardShortcut(KeyCode.Insert));
            KeybindToggleCameraOverride = Config.Bind("Keybinds", "KeybindToggleCameraOverride", new KeyboardShortcut(KeyCode.PageUp));
        }
    }
}