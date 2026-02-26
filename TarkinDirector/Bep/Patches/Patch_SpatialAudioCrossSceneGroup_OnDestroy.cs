using Audio.SpatialSystem;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

namespace tarkin.Director.Bep.Patches
{
    internal class Patch_SpatialAudioCrossSceneGroup_OnDestroy : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SpatialAudioCrossSceneGroup), nameof(SpatialAudioCrossSceneGroup.OnDestroy));
        }

        [PatchPrefix]
        private static bool PatchPrefix(SpatialAudioCrossSceneGroup __instance)
        {
            SpatialAudioCrossSceneGroup.AllCrossGroups.Remove(__instance);

            return false;
        }
    }
}
