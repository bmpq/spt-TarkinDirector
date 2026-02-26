using Audio.SpatialSystem;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

namespace tarkin.Director.Bep.Patches
{
    internal class Patch_SpatialAudioCrossSceneGroup_Awake : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SpatialAudioCrossSceneGroup), nameof(SpatialAudioCrossSceneGroup.Awake));
        }

        [PatchPostfix]
        private static void PatchPostfix(SpatialAudioCrossSceneGroup __instance)
        {
            Patch_AudioRoomTracker_RegisterAllRooms.CurrentAudioRoomTracker.method_0();
#if !SPT3_11
            Patch_AudioRoomTracker_RegisterAllRooms.CurrentAudioRoomTracker.method_1();
#endif
        }
    }
}
