using EFT;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using UnityEngine;

namespace tarkin.BSP.Bep.Patches
{
    internal class Patch_EnvironmentUIRoot_SetCameraActive : ModulePatch
    {
        public static EnvironmentUIRoot CurrentEnvironmentUIRoot { get; private set; }

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(EnvironmentUIRoot), nameof(EnvironmentUIRoot.SetCameraActive));
        }

        [PatchPostfix]
        private static void PatchPostfix(EnvironmentUIRoot __instance)
        {
            CurrentEnvironmentUIRoot = __instance;
            Plugin.Log.LogInfo($"{CurrentEnvironmentUIRoot.name}");
        }
    }
}
