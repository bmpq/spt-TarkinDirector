using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using tarkin.Director.EFTRuntime;
using tarkin.Director.Bep.Patches;

namespace tarkin.Director.Bep
{
    [BepInPlugin("com.tarkin.bundlesceneplayer", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Log;

        internal static ConfigEntry<string> BundleName;
        internal static ConfigEntry<float> CameraOverrideHandoverSpeed;
        internal static ConfigEntry<KeyboardShortcut> KeybindPlayback;
        internal static ConfigEntry<KeyboardShortcut> KeybindToggleCameraOverride;
        internal static ConfigEntry<KeyboardShortcut> KeybindUnloadAll;

        internal static ConfigEntry<string> PrewarmAssemblies;
        internal static ConfigEntry<bool> Silent;
        internal static ConfigEntry<bool> CleanDecals;
        internal static ConfigEntry<bool> SetActiveScene;

        private const int MAX_BUNDLE_SLOTS = 5;
        internal static List<ConfigEntry<string>> BundleSlots = new List<ConfigEntry<string>>();

        public static List<string> GetConfiguredBundlePaths()
        {
            var paths = new List<string>();
            foreach (var slot in BundleSlots)
            {
                if (!string.IsNullOrWhiteSpace(slot.Value))
                {
                    paths.Add(Path.Combine(BepInEx.Paths.PluginPath, "tarkin", "bundles", slot.Value));
                }
            }
            return paths;
        }

        private void Start()
        {
            new Patch_InteractionContextHelper_GetAvailableActions().Enable();

            InitConfiguration();

            Log = base.Logger;

            DontDestroyOnLoad(new GameObject("Bundle Scene Player").AddComponent<BundleScenePlayer>().gameObject);

            new Patch_Door_KickOpen().Enable();
            new Patch_WorldInteractiveObject_DoorStateChanged().Enable();
            new Patch_TripwireSynchronizableObject_SetupGrenade().Enable();

            new Patch_AudioRoomTracker_RegisterAllRooms().Enable();
            new Patch_SpatialAudioCrossSceneGroup_OnDestroy().Enable();

            new Patch_EnvironmentUIRoot_SetCameraActive().Enable();
        }

        private void InitConfiguration()
        {
            for (int i = 0; i < MAX_BUNDLE_SLOTS; i++)
            {
                int slotNum = i + 1;
                string defaultValue = "";

                var entry = Config.Bind("Bundle Slots", $"Bundle {slotNum}", defaultValue,
                    $"Name of bundle file #{slotNum} to load.");

                BundleSlots.Add(entry);
            }

            PrewarmAssemblies = Config.Bind("General", "PrewarmAssemblies", "", 
                "Prewarm assemblies separated by a comma, without extention. " +
                "Prewarming is required when a scene contains serialized references to that assembly, since Unity doesn't load them automatically.");

            CameraOverrideHandoverSpeed = Config.Bind("General", "CameraOverrideHandoverSpeed", 2f, "");

            KeybindPlayback = Config.Bind("Keybinds", "Keybind Playback", new KeyboardShortcut(KeyCode.Insert));
            KeybindUnloadAll = Config.Bind("Keybinds", "Keybind Unload All", new KeyboardShortcut(KeyCode.Delete));
            KeybindToggleCameraOverride = Config.Bind("Keybinds", "KeybindToggleCameraOverride", new KeyboardShortcut(KeyCode.PageUp));

            Silent = Config.Bind("General", "Silent", false);
            CleanDecals = Config.Bind("General", "CleanDecalsOnLoad", true);
            SetActiveScene = Config.Bind("General", "SetActiveScene", false);
        }
    }
}