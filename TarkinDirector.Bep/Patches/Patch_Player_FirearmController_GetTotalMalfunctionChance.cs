using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace tarkin.Director.Bep.Patches
{
    internal class Patch_Player_FirearmController_GetTotalMalfunctionChance : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player.FirearmController), nameof(Player.FirearmController.GetTotalMalfunctionChance));
        }


        [PatchPrefix]
        private static bool PatchPrefix(ref object __instance, ref float __result)
        {
            if (!Plugin.OverrideMalfunctionChance.Value)
                return true;

            __result = Plugin.OverrideMalfunctionChanceFactor.Value;
            return false;
        }
    }
}
