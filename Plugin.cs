using BepInEx;
using BepInEx.Logging;

namespace tarkin.bundlesceneplayer
{
    [BepInPlugin("com.tarkin.bundlesceneplayer", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Log;

        private void Awake()
        {
            Log = base.Logger;

            InitConfiguration();
        }

        private void InitConfiguration()
        {
        }
    }
}