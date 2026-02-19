using Comfort.Common;
using EFT.Interactive;
using Systems.Effects;
using UnityEngine;

namespace tarkin.BSP.Bep
{
    public static class SceneRuntimeUtils
    {
        public static void ClearDeadBodies()
        {
            foreach (var corpse in UnityEngine.Object.FindObjectsByType<Corpse>(FindObjectsSortMode.None))
            {
                corpse.Kill();
            }
        }

        public static void ClearDecals()
        {
            Singleton<Effects>.Instance.ClearDecal();
        }
    }
}
