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

        public static void HideDeadBodies()
        {
            foreach (var corpse in UnityEngine.Object.FindObjectsByType<Corpse>(FindObjectsSortMode.None))
            {
                corpse.transform.position = new Vector3(0, -100, 0);
                corpse.RemoveLootItem(new GEventArgs3(corpse.ItemInHands, null, EFT.InventoryLogic.CommandStatus.Succeed, null));
            }
        }

        public static void ClearDecals()
        {
            Singleton<Effects>.Instance.ClearDecal();
        }
    }
}
