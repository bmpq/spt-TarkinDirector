using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.IO;
using UnityEngine;

namespace tarkin.bundlesceneplayer
{
    [BepInPlugin("com.tarkin.bundlesceneplayer", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        public static string AddPathToApplicationDataPath = Path.Combine("BepInEx", "plugins", "tarkin", "bundles");

        internal static new ManualLogSource Log;

        internal static ConfigEntry<string> BundleName;
        internal static ConfigEntry<KeyboardShortcut> KeybindPlayback;

        private void Awake()
        {
            Log = base.Logger;

            InitConfiguration();

            DontDestroyOnLoad(new GameObject("Bundle Scene Player").AddComponent<BundleScenePlayer>().gameObject);
        }

        private void InitConfiguration()
        {
            BundleName = Config.Bind("General", "Bundle name", "scene_buckshot", "");

            KeybindPlayback = Config.Bind("Keybinds", "Keybind Playback", new KeyboardShortcut(KeyCode.Insert));
        }
    }
}