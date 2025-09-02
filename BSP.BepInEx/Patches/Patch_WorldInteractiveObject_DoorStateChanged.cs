using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using UnityEngine;

namespace tarkin.BSP.Bep.Patches
{
    internal class Patch_WorldInteractiveObject_DoorStateChanged : ModulePatch
    {
        public static event Action<EDoorState> OnPostfix;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(WorldInteractiveObject), nameof(WorldInteractiveObject.DoorStateChanged));
        }

        [PatchPostfix]
        private static void PatchPostfix(WorldInteractiveObject __instance, EDoorState newState)
        {
            OnPostfix?.Invoke(newState);
        }
    }
}
