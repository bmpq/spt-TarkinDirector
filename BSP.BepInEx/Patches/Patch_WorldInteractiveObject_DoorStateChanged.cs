using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using tarkin.BSP.Shared;
using UnityEngine;

namespace tarkin.BSP.Bep.Patches
{
    internal class Patch_WorldInteractiveObject_DoorStateChanged : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(WorldInteractiveObject), nameof(WorldInteractiveObject.DoorStateChanged));
        }

        [PatchPostfix]
        private static void PatchPostfix(WorldInteractiveObject __instance, EDoorState newState)
        {
            if (newState == EDoorState.Open)
                EFTTrigger.SendEFTEvent(EFTTrigger.Trigger.DoorOpen);

            else if (newState == EDoorState.Shut)
                EFTTrigger.SendEFTEvent(EFTTrigger.Trigger.DoorShut);
        }
    }
}
