using EFT;
using EFT.Interactive;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using UnityEngine;

namespace tarkin.BSP.Bep.Patches
{
    internal class Patch_Door_KickOpen : ModulePatch
    {
        public static event Action OnPostfix;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Door), nameof(Door.KickOpen), [typeof(Vector3), typeof(bool)]);
        }

        [PatchPostfix]
        private static void PatchPostfix(Door __instance, Vector3 yourPosition)
        {
            OnPostfix?.Invoke();
        }
    }
}
