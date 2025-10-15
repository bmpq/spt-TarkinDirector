using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.IO;
using System.Linq;
using System.Reflection;
using tarkin.BSP.Bep.Patches;
using UnityEngine;
using tarkin;
using tarkin.SimpleTransformAnimation.Player;
using tarkin.SimpleTransformAnimation.Format;
using tarkin.BSP.Bep.Mediators;

namespace tarkin.BSP.Bep
{
    [BepInPlugin("com.tarkin.bundlesceneplayer", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin
    {
        public static string AddAssembliesPathToPluginPath = Path.Combine("tarkin");
        public static string AddBundlesPathToPluginPath = Path.Combine("tarkin", "bundles", "bsp_scenes");

        internal static new ManualLogSource Log;

        internal static ConfigEntry<string> BundleName;
        internal static ConfigEntry<float> CameraOverrideHandoverSpeed;
        internal static ConfigEntry<KeyboardShortcut> KeybindPlayback;
        internal static ConfigEntry<KeyboardShortcut> KeybindToggleCameraOverride;
        internal static ConfigEntry<KeyboardShortcut> KeybindUnloadAll;

        internal static ConfigEntry<string> PrewarmAssemblies;
        internal static ConfigEntry<bool> Silent;
        internal static ConfigEntry<bool> CleanDecals;

        public static string BundleFullPath => Path.Combine(BepInEx.Paths.PluginPath, AddBundlesPathToPluginPath, BundleName.Value);

        private void Awake()
        {
            var prewarm = (typeof(BoneMapping), typeof(NodeData));
            InitConfiguration();

            Log = base.Logger;

            DontDestroyOnLoad(new GameObject("Bundle Scene Player").AddComponent<BundleScenePlayer>().gameObject);

            new MovingPlatformMediator();
            new EFTPersistentAudioSourceHandler();
            new SmokeGrenadeMediator();
            new TripwireMediator();
            new HurtBoxMediator();
            new EffectEmitterMediator();

            new Patch_Door_KickOpen().Enable();
            new Patch_WorldInteractiveObject_DoorStateChanged().Enable();
            new Patch_TripwireSynchronizableObject_SetupGrenade().Enable();
        }

        private void InitConfiguration()
        {
            BundleName = Config.Bind("General", "Bundle name", "scene_buckshot", "");

            PrewarmAssemblies = Config.Bind("General", "PrewarmAssemblies", "", 
                "Prewarm assemblies separated by a comma, without extention. " +
                "Prewarming is required when a scene contains serialized references to that assembly, since Unity doesn't load them automatically.");

            CameraOverrideHandoverSpeed = Config.Bind("General", "CameraOverrideHandoverSpeed", 2f, "");

            KeybindPlayback = Config.Bind("Keybinds", "Keybind Playback", new KeyboardShortcut(KeyCode.Insert));
            KeybindUnloadAll = Config.Bind("Keybinds", "Keybind Unload All", new KeyboardShortcut(KeyCode.Delete));
            KeybindToggleCameraOverride = Config.Bind("Keybinds", "KeybindToggleCameraOverride", new KeyboardShortcut(KeyCode.PageUp));

            Silent = Config.Bind("General", "Silent", false);
            CleanDecals = Config.Bind("General", "CleanDecalsOnLoad", true);
        }
    }
}