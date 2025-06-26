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
        internal static ConfigEntry<KeyboardShortcut> KeybindPlayback;
        internal static ConfigEntry<KeyboardShortcut> KeybindReleaseAnimatedCamera;

        internal static ConfigEntry<PlaybackTrigger> Trigger;

        private void Awake()
        {
            Log = base.Logger;

            InitConfiguration();

            DontDestroyOnLoad(new GameObject("Bundle Scene Player").AddComponent<BundleScenePlayer>().gameObject);

            new Patch_Door_KickOpen().Enable();
        }

        private void InitConfiguration()
        {
            BundleName = Config.Bind("General", "Bundle name", "scene_buckshot", "");

            KeybindPlayback = Config.Bind("Keybinds", "Keybind Playback", new KeyboardShortcut(KeyCode.Insert));
            KeybindReleaseAnimatedCamera = Config.Bind("Keybinds", "KeybindReleaseAnimatedCamera", new KeyboardShortcut(KeyCode.PageUp));

            Trigger = Config.Bind("General", "PlaybackTrigger", PlaybackTrigger.KeybindOnly);
        }
    }

    internal enum PlaybackTrigger
    {
        KeybindOnly,
        DoorBreach
    }
}